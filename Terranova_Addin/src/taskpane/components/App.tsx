/* eslint-disable prettier/prettier */
/* eslint-disable react/no-direct-mutation-state */
/* eslint-disable no-undef */
import * as React from "react";
import { v4 as uuidv4 } from 'uuid';
import Progress from "./Common/Progress";
import MessageInfoBar from "./Common/MessageInfoBar";
import Confirmation from "./UI/Confirmation";
import TypeSelectionCommentBox from "./UI/TypeSelectionCommentBox";
import Move from "./UI/Move";
import Authorize from "./UI/Authorize";
import AuthorizeError from "./UI/AuthorizeError";
import { getAuthToken } from "../services/MobileClientAuthTokenHandler";
import { getAccessToken } from "../services/MobileClientAuthRefreshTokenHandler";
import { authenticate } from "../services/AuthHandler";
import { getConfigData } from "../services/ConfigDataHandler";
import { getPreviouslyReportedStatus, reportEmail } from "../services/EmailReportHandler";
import { moveEmail } from "../services/EmailMoveHandler";
import Configuration from "../models/Configuration";
import TypeSelection from "../models/TypeSelection";
import Payload from "../models/Payload";
import ReportEmailResponse from "../models/ReportEmailResponse";
import { MessageText } from "../utils/constants/MessageText";
import { MessageTypes } from "../utils/constants/MessageTypes";
import { ApiUrls } from "../environments/ApiUrls";
import { getMSAuthUrl, getRefreshTokenForGuid } from "../services/IntuneConfiguredMobileClientAuthHandler";
import { getUrlParamsValues } from "../utils/helpers/CommonHelper";
import { UrlParams } from "../models/UrlParams";
import { LocalStorageItemNames } from "../utils/constants/LocalStorageItemNames";
import { OutlookHostNames } from "../utils/constants/OutlookHostNames";
import { CommonIdentifiers } from "../utils/constants/CommonIdentifiers";
import IntuneConfiguredIOSAuthorize from "./UI/IntuneConfiguredIOSAuthorize";
import IntuneConfiguredAndroidAuthorize from "./UI/IntuneConfiguredAndroidAuthorize";
import { UrlParamNames } from "../utils/constants/UrlParamNames";

let dialog: any;
let intervalId;

export interface AppProps {
  isOfficeInitialized: boolean;
  title: string;
}

export interface AppState {
  isTokenAuthAvailable: boolean;
  isRefreshTokenInvalid: boolean;
  isUserVerified: boolean;
  isError: boolean;
  isConfirmationScreenCompleted: boolean;
  isTypeSelectionCommentBoxScreenCompleted: boolean;
  isSimulationCheckCompleted: boolean;
  isPreviouslyReported: boolean;
  isReported: boolean;
  isMoveButtonPressed: boolean;
  isEmailInJunkOrDeletedItemsFolder: boolean;
  errorMessage: string;
  comment: string;
  feedbackMessage: string;
  accessToken: string;
  configuration: Configuration;
  selectedEmailType: TypeSelection;
  isSimulation: boolean;
  moveButtonCaption: string;
  msAuthUrl: string;
  isAuthorizationInProgress: boolean;
  urlParams: UrlParams
}

export default class App extends React.Component<AppProps, AppState> {
  constructor(props: AppProps, context: any) {
    super(props, context);
    this.state = {
      isTokenAuthAvailable: false,
      isRefreshTokenInvalid: false,
      isUserVerified: false,
      isError: false,
      isConfirmationScreenCompleted: false,
      isTypeSelectionCommentBoxScreenCompleted: false,
      isSimulationCheckCompleted: false,
      isPreviouslyReported: null,
      isReported: false,
      isMoveButtonPressed: false,
      isEmailInJunkOrDeletedItemsFolder: false,
      errorMessage: "",
      comment: "",
      feedbackMessage: "",
      accessToken: "",
      configuration: null,
      selectedEmailType: null,
      isSimulation: false,
      moveButtonCaption: "",
      msAuthUrl: "",
      isAuthorizationInProgress: false,
      urlParams: {
        environmentId: "",
        isIntuneConfigured: false,
        code: "",
        state: ""
      }
    };
  }

  async componentDidMount() {
    try {
      if (this.props.isOfficeInitialized) {
        await this.setUrlParams();
        await this.getAccessTokenFromAuthCodeInURLParams();
        await this.getConfig();
        await this.verifyUser();
      }
    } catch (ex) {
      console.log(`App.tsx : ${ex.message}`);
      this.showError(ex.message);
    }
  }

  setUrlParams = async () => {
    var urlParams = await getUrlParamsValues();
    console.log(urlParams);
    console.log(Office.context.displayLanguage);
    this.setState({ urlParams: urlParams });
  }

  timer = async () => {
    if (this.state.urlParams.isIntuneConfigured && this.displayDialogAuthorizationScreen() && this.state.isAuthorizationInProgress) {
      this.setState({ isAuthorizationInProgress: true });
      let tokenRetrievalGuid = localStorage.getItem(LocalStorageItemNames.TOKEN_RETRIEVAL_GUID);
      if (tokenRetrievalGuid) {
        var refreshToken = await getRefreshTokenForGuid(tokenRetrievalGuid, this.showError);
        if (refreshToken) {
          localStorage.setItem(LocalStorageItemNames.REFRESH_TOKEN, refreshToken);
          localStorage.removeItem(LocalStorageItemNames.TOKEN_RETRIEVAL_GUID);
          clearInterval(intervalId);
          await this.verifyUser();
          this.setState({ isAuthorizationInProgress: false });
        }
      }
    }
    else {
      clearInterval(intervalId);
    }
  };

  showError = (error: string) => {
    this.setState({
      isError: true,
      errorMessage: error,
    });
  };

  getAccessTokenFromAuthCodeInURLParams = async () => {
    try {
      let urlParams = window.location.search;

      if (urlParams.includes(UrlParamNames.STATE)) {
        if (urlParams.includes(UrlParamNames.CODE)) {
          let paramsArray = urlParams.substring(1).split("&");
          let code = decodeURIComponent(paramsArray.find((x) => x.startsWith(UrlParamNames.CODE))).split("=")[1];
          console.log(`App.tsx - getAccessTokenFromAuthCodeInURLParams : ${code}`);
          await getAuthToken(code, this.showError, this.closeDialogBox);
        } else {
          this.showError(MessageText.noCodeFoundInDialogBoxUrl);
        }
      }

    } catch (ex) {
      this.showError(MessageText.invalidCodeFoundInDialogBoxUrl);
    }
  };

  closeDialogBox = async () => {
    Office.context.ui.messageParent(JSON.stringify({ messageType: "signinSuccess" }));
  };

  getConfig = async () => {
      let displayLanguage = Office.context.displayLanguage;
      console.log(displayLanguage);
      await getConfigData(
        this.state.urlParams.environmentId,
        displayLanguage,
        this.setConfigData,
        this.showError
      );
    
  };

  verifyUser = async () => {
    if (this.isIdentityAPIRequrementSetSupported() && typeof OfficeRuntime != "undefined") {
      let bootstrapToken = await OfficeRuntime.auth.getAccessToken();
      console.log(`Token: ${bootstrapToken}`);
      await authenticate(bootstrapToken, this.handleAuthError, this.updateUserVerification);
    }
    else {
      if (localStorage.getItem(LocalStorageItemNames.REFRESH_TOKEN)) {
        //Refresh the token
        console.log("OfficeRuntime is not available. Using Token based authorization...");
        await getAccessToken(localStorage.getItem(LocalStorageItemNames.REFRESH_TOKEN), this.setAccessToken, this.setRefreshTokenAsInvalid);
        await authenticate(this.state.accessToken, this.handleAuthError, this.updateUserVerification);
      } else {
        //render authorize component
        this.setState({
          isTokenAuthAvailable: true,
        });
      }
    }
  };

  handleAuthError = (error: string, isInvalidClient: boolean = false,) => {
    if (isInvalidClient) {
      localStorage.removeItem(LocalStorageItemNames.REFRESH_TOKEN);
      this.setState({ isUserVerified: false, isRefreshTokenInvalid: true, isAuthorizationInProgress: false });
    }
    else {
      this.showError(error);
    }
  }

  setAccessToken = (accessToken: string) => {
    this.setState({
      accessToken: accessToken,
    });
  };

  setRefreshTokenAsInvalid = () => {
    this.setState({
      isRefreshTokenInvalid: true,
    });
  };

  updateUserVerification = () => {
    this.setState({
      isUserVerified: true,
    });
    this.checkPreviouslyReportedStatus();
  };

  setPreviouslyReportedStatus = (status: boolean) => {
    this.setState({
      isPreviouslyReported: status,
    });
  }

  setConfigData = (configData: Configuration) => {
    this.setState({
      configuration: configData,
    });
  };

  onClickConfirmation = () => {
    console.log("Clicked");
    this.setState({
      isConfirmationScreenCompleted: true,
    });
  };

  onClickForDialogAuthorization = () => {
    this.setState({ isAuthorizationInProgress: true });
    this.clearErrorForReAuthenticate();
    Office.context.ui.displayDialogAsync(
      `${ApiUrls.urlAuthHTMLPrefix}${ApiUrls.urlGetMSRedirectUrl}`,
      {
        height: 70,
        width: 42,
        displayInIframe: false,
        promptBeforeOpen: true,
      },
      (asyncResult) => {
        if (asyncResult.status === Office.AsyncResultStatus.Succeeded) {
          dialog = asyncResult.value;
          console.log("dialog- " + dialog);
          dialog.addEventHandler(Office.EventType.DialogMessageReceived, async (arg: { message: string }) => {
            var messageFromDialog = JSON.parse(arg.message);
            console.log(messageFromDialog);
            if (messageFromDialog.messageType === "signinSuccess") {
              dialog.close();
              await getAccessToken(
                localStorage.getItem(LocalStorageItemNames.REFRESH_TOKEN),
                this.setAccessToken,
                this.setRefreshTokenAsInvalid
              );
              await authenticate(this.state.accessToken, this.handleAuthError, this.updateUserVerification);
              this.setState({ isAuthorizationInProgress: false });
            }
          });
        } else {
          console.log("Dialog failed");
        }
      }
    );
  };

  openDialogApiForAndroidAuth = () => {
    this.clearErrorForReAuthenticate();
    const { urlParams } = this.state;
    Office.context.ui.displayDialogAsync(
      `${ApiUrls.urlAndroidAuthHTMLPrefix}${ApiUrls.urlGetMSRedirectUrl}&tokenRetrievalGuid=${uuidv4()}&isIntuneConfigured=${urlParams.isIntuneConfigured}`,
      {
        height: 70,
        width: 42,
        displayInIframe: false,
        promptBeforeOpen: true,
      },
      (asyncResult) => {
        if (asyncResult.status === Office.AsyncResultStatus.Succeeded) {
          dialog = asyncResult.value;
          console.log("dialog- " + dialog);
          dialog.addEventHandler(Office.EventType.DialogMessageReceived, async (arg: { message: string }) => {
            var messageFromDialog = JSON.parse(arg.message);
            console.log(messageFromDialog);
            if (messageFromDialog.messageType === "signinSuccess") {
              dialog.close();
              this.setState({ isAuthorizationInProgress: true });
              localStorage.setItem(LocalStorageItemNames.TOKEN_RETRIEVAL_GUID, messageFromDialog.addinGuid);
              intervalId = setInterval(this.timer, 3000);
            }
          });
        } else {
          console.log("Dialog failed");
        }
      }
    );
  }

  onClickGetTypeSelectionAndComment = (typeSelection: TypeSelection, comment: string) => {
    this.setState({
      isTypeSelectionCommentBoxScreenCompleted: true,
      selectedEmailType: typeSelection,
      comment: comment,
    });
  };

  onClickMove = () => {
    this.setState({
      isMoveButtonPressed: true,
    });
  };

  setReportStatus = (response: ReportEmailResponse) => {
    this.setState({
      feedbackMessage: response.feedbackMessage,
      isEmailInJunkOrDeletedItemsFolder: response.isMailInJunkOrDeletedItemsFolder,
      isReported: true,
      isSimulation: response.isSimulation,
    });
    this.addReportedItemToLocalStorage();
  };

  addReportedItemToLocalStorage = () => {
    var currentItemId = Office.context.mailbox.convertToRestId(
      Office.context.mailbox.item.itemId,
      Office.MailboxEnums.RestVersion.v2_0
    );
    var reportedItems = localStorage.getItem('reportedItems');
    var reportedItemsArray = reportedItems ? JSON.parse(reportedItems) : [];
    reportedItemsArray.push(currentItemId);
    localStorage.setItem('reportedItems', JSON.stringify(reportedItemsArray));
  }

  isMessageIncludesInLocalStoragePreviouslyReportedItems = (mailItemId : string): boolean => {
    var perviouslyReportedItems = localStorage.getItem('reportedItems');
    if (perviouslyReportedItems){
      var perviouslyReportedItemsArray = JSON.parse(perviouslyReportedItems);
      if(perviouslyReportedItemsArray.includes(mailItemId)){
        return true;
      }
      return false;
    } 
    return false;
  }

  isEmailReadyToMove = () => {
    // if email is in Inbox or Sent Items & move button pressed => email ready to move
    // if email is in Inbox or Sent Items & displayFeedbackScreen = false => email ready to move automatically
    if(!this.state.isEmailInJunkOrDeletedItemsFolder && (this.state.isMoveButtonPressed || !this.state.configuration.displayFeedbackScreen))
    {
      return true
    } else return false;
  }

  getMoveButtonCaption = () => {
    if(this.state.isSimulation){ 
        return this.state.configuration.moveSimulationEmailsToJunk ? this.state.configuration.moveToJunkButtonCaption : this.state.configuration.moveToDeletedItemsButtonCaption      
    }else{      
        return this.state.configuration.moveNonSimulationEmailsToJunk ? this.state.configuration.moveToJunkButtonCaption : this.state.configuration.moveToDeletedItemsButtonCaption    
    }
  }

  getEmailMovedSuccessMessageText = () => {
    if(this.state.isSimulation){ 
        return this.state.configuration.moveSimulationEmailsToJunk ? this.state.configuration.movedToJunkText : this.state.configuration.movedToDeletedItemsText      
    }else{      
        return this.state.configuration.moveNonSimulationEmailsToJunk ? this.state.configuration.movedToJunkText : this.state.configuration.movedToDeletedItemsText    
    }
  }

  setEmailMoveStatus = () => {
    this.setState({
      isEmailInJunkOrDeletedItemsFolder: true,
    });
  };

  displayDialogAuthorizationScreen = (): boolean => {
    if (this.state.isTokenAuthAvailable && !this.state.isUserVerified) return true;
    else if (this.state.isRefreshTokenInvalid && !this.state.isUserVerified) return true;
    else return false;
  };

  checkConfirmationScreen = (configData: Configuration): boolean => {
    if (!configData.displayConfirmation) return false;
    else if (this.state.isConfirmationScreenCompleted) return false;
    else return true;
  };

  checkTypeSelectionCommentBoxScreen = (configData: Configuration): boolean => {
    if ((!configData.promptUserForTypeSelection) && (!configData.commentBoxEnabled)) return false;
    else if (this.state.isTypeSelectionCommentBoxScreenCompleted) return false;
    else return true;
  };

  checkPreviouslyReportedStatus = async () => {
    try {
      let mailItemId = this.getMailItemRestId();
      if(this.isMessageIncludesInLocalStoragePreviouslyReportedItems(mailItemId)){
        this.setPreviouslyReportedStatus(true);
      } else {
          let token: string = "";
          if (this.isIdentityAPIRequrementSetSupported() && typeof OfficeRuntime != "undefined") {
            token = await OfficeRuntime.auth.getAccessToken();
          } else {
            token = this.state.accessToken;
          }
          let payload = new Payload();
          payload.mailItemId = mailItemId;
          payload.mailboxAddress = await this.getMailboxAddress(token);
          await getPreviouslyReportedStatus(token, this.setPreviouslyReportedStatus, this.showError, payload);
      }
    } catch (error) {
      console.log(`App.tsx - reportedStatusInfo : ${error.message}`);
      this.showError(error.message);
    }
  }

  reportEmail = async () => {
    try {
      let token: string = "";
      if (this.isIdentityAPIRequrementSetSupported() && typeof OfficeRuntime != "undefined") {
        token = await OfficeRuntime.auth.getAccessToken();
      } else {
        token = this.state.accessToken;
      }
      let mailItemId = this.getMailItemRestId();
      const { configuration, selectedEmailType, comment, urlParams } = this.state;
      let payload = new Payload();
      payload.mailItemId = mailItemId;
      payload.environmentId = urlParams.environmentId;
      payload.mailboxAddress = await this.getMailboxAddress(token);
      if (configuration) payload.configurationInfo = configuration;
      if (this.state.isTypeSelectionCommentBoxScreenCompleted) {
        payload.feedbackMessage = comment;
        payload.emailType = selectedEmailType;
      } else {
        payload.emailType = {
          key: 1,
          value: "Spam",
        };
      }
      await reportEmail(token, this.setReportStatus, this.showError, payload);
    } catch (error) {
      console.log(`App.tsx - reportEmailInfo : ${error.message}`);
      this.showError(error.message);
    }
  };

  moveEmail = async () => {
    try {
      let token: string = "";
      if (this.isIdentityAPIRequrementSetSupported() && typeof OfficeRuntime != "undefined") {
        token = await OfficeRuntime.auth.getAccessToken();
      } else {
        token = this.state.accessToken;
      }
      let payload = new Payload();
      payload.mailItemId = this.getMailItemRestId();
      payload.mailboxAddress = await this.getMailboxAddress(token);
      payload.configurationInfo = await this.state.configuration;
      await moveEmail(token, this.showError, this.setEmailMoveStatus, payload, this.state.isSimulation);
    } catch (error) {
      console.log(`App.tsx - reportEmailInfo : ${error.message}`);
      this.showError(error.message);
    }
  };

  getMailItemRestId = (): string => {
    if (
      Office.context.mailbox.diagnostics.hostName === OutlookHostNames.IOS ||
      Office.context.mailbox.diagnostics.hostName === OutlookHostNames.ANDROID
    ) {
      // itemId is already REST-formatted.
      return Office.context.mailbox.item.itemId;
    } else {
      // Convert to an item ID for API v2.0.
      return Office.context.mailbox.convertToRestId(
        Office.context.mailbox.item.itemId,
        Office.MailboxEnums.RestVersion.v2_0
      );
    }
  };

  getSharedMailbox = async (token: string) => {
    return new Promise<any>((resolve, reject) => {
      Office.context.mailbox.item.getSharedPropertiesAsync({ asyncContext: token }, (asyncResult) => {
        if (asyncResult.status === Office.AsyncResultStatus.Succeeded && asyncResult.value) {
          let sharedProperties = asyncResult.value;
          resolve(sharedProperties.targetMailbox);
        } else {
          reject(this.showError(asyncResult.error.message));
        }
      });
    });
  };

  getMailboxAddress = async (token: string): Promise<string> => {
    return Office.context.mailbox.item.getSharedPropertiesAsync ? await this.getSharedMailbox(token) : Office.context.mailbox.userProfile.emailAddress;
  };

  setAuthorizingStatus = () => {
    this.setState({ isAuthorizationInProgress: true });
    intervalId = setInterval(this.timer, 3000);
  }

  getMSAuthUrl = async () => {
    let guidVal = uuidv4();
    localStorage.setItem(LocalStorageItemNames.TOKEN_RETRIEVAL_GUID, guidVal);
    //get the ms auth url if device is ios
    return await getMSAuthUrl(guidVal, this.showError);
  }

  setMsAuthUrl = (msAuthUrl: string) => {
    this.setState({ msAuthUrl: msAuthUrl });
  }

  isIdentityAPIRequrementSetSupported = () => {
    if (Office.context.requirements.isSetSupported('IdentityAPI', '1.3')) {
      return true;
    }
    else {
      return false;
    }
  }

  // check any Azure Active Directory Security Token Service isuue
  isAADSTSAuthError = (errorMessage: string) => {
    if(errorMessage.includes(CommonIdentifiers.AAD_AUTH_ERROR_IDENTIFIER)){     
      return true;
    } else return false;
  }

  clearErrorForReAuthenticate = () => {
    this.setState({
      isError: false,
      errorMessage: "",
    });
  }

  render() {
    const { isOfficeInitialized } = this.props;
    let count = window.localStorage.getItem(LocalStorageItemNames.REFRESH_COUNT);

    if (count !== null && +count >= 3) {
      return <MessageInfoBar message={MessageText.refreshAddinMessage} messageType={MessageTypes.info} />;
    } else {
      if (!isOfficeInitialized) {
        return <Progress message={MessageText.sideLoadAddinMessage} />;
      }

      if (this.state.isError) {
        if(this.isAADSTSAuthError(this.state.errorMessage)){
          return <AuthorizeError 
            isIntuneConfigured={this.state.urlParams.isIntuneConfigured}
            authError={this.state.errorMessage} 
            hostName={Office.context.mailbox.diagnostics.hostName}
            configData={this.state.configuration}
            clearErrorForReAuthenticate={this.clearErrorForReAuthenticate}
            setAuthorizingStatus={this.setAuthorizingStatus}
            getMSAuthUrl={this.getMSAuthUrl}
            openDialogApiForAndroidAuth={this.openDialogApiForAndroidAuth}
            onClickForDialogAuthorization={this.onClickForDialogAuthorization}>
          </AuthorizeError>          
        } else{
          return (
            <MessageInfoBar message={this.state.errorMessage} messageType={MessageTypes.error} />
          );
        }
      } else {
        const { configuration } = this.state;
        console.log(configuration);
        if(configuration){       
        if (this.state.isAuthorizationInProgress) {
          return <Progress message={configuration.authorizingLabelText} />;
        }
        let dialogAuthorizationScreenPending = this.displayDialogAuthorizationScreen();
        if (dialogAuthorizationScreenPending) {
          if (this.state.urlParams.isIntuneConfigured && Office.context.mailbox.diagnostics.hostName === OutlookHostNames.IOS) {          
              return <IntuneConfiguredIOSAuthorize
                setAuthorizingStatus={this.setAuthorizingStatus}
                getMSAuthUrl={this.getMSAuthUrl}
                configData={configuration} 
                clearErrorForReAuthenticate={this.clearErrorForReAuthenticate}//Purpose of passing "clearErrorForReAuthenticate" function is only to avoid missing property type error 
                authError={""}>
              </IntuneConfiguredIOSAuthorize>  
          }
          else if (this.state.urlParams.isIntuneConfigured && Office.context.mailbox.diagnostics.hostName === OutlookHostNames.ANDROID) {
            return <IntuneConfiguredAndroidAuthorize
              openDialogAPIForAndroidAuth={this.openDialogApiForAndroidAuth}
              configData={configuration}
              authError={""}>
            </IntuneConfiguredAndroidAuthorize>
          }
          else {
            return <Authorize
              onClickAuthorizeHandler={this.onClickForDialogAuthorization}
              configData={configuration}
              authError={""}
            />;
          }
        }
        if (this.state.isUserVerified) {      
          if(this.state.isPreviouslyReported === false){
            let confirmationPending = this.checkConfirmationScreen(configuration);
            let typeSelectionCommentBoxPending = this.checkTypeSelectionCommentBoxScreen(configuration);
            if (configuration.displayConfirmation && !this.state.isConfirmationScreenCompleted) {
              return (
                <Confirmation
                  configuration={configuration}
                  onNextClickHandler={this.onClickConfirmation}
                />
              );
            }

            if ((configuration.promptUserForTypeSelection || configuration.commentBoxEnabled) && typeSelectionCommentBoxPending && !confirmationPending) {
              return (
                <TypeSelectionCommentBox
                  configuration={configuration}
                  getTypeSelectionAndComment={this.onClickGetTypeSelectionAndComment}
                />
              );
            }

            if (!confirmationPending && !typeSelectionCommentBoxPending && !this.state.isReported) {
              this.reportEmail();
              return <Progress message={configuration.reportingText} />;
            }

            if (this.state.isReported && !this.state.isMoveButtonPressed && configuration.displayFeedbackScreen) {
                return (
                  <Move
                    feedbackMessage={this.state.feedbackMessage}
                    moveButtonCaption={this.getMoveButtonCaption()}
                    moveButtonBackgroundColor={configuration.moveButtonBackgroundColor}
                    moveButtonTextColor={configuration.moveButtonForegroundColor}
                    isEmailInJunkOrDeletedItemsFolder={this.state.isEmailInJunkOrDeletedItemsFolder}
                    onMoveClickHandler={this.onClickMove}
                  />
                );              
            }

            if (this.isEmailReadyToMove()) {
              this.moveEmail();
              return <Progress message={configuration.movingText} />;
            }

            if (this.state.isEmailInJunkOrDeletedItemsFolder) {
                return (
                  <MessageInfoBar message={this.getEmailMovedSuccessMessageText()} messageType={MessageTypes.success} />);             
            }

          } else if(this.state.isPreviouslyReported === true) {
            return (
              <MessageInfoBar message={MessageText.alreadyReportedMesasage} messageType={MessageTypes.info} />);
          }
            return <Progress message={configuration.loadingLabelText} />;
        } else {
          return <Progress message={configuration.validatingAuthText} />;
        }
      } else{
        return <Progress message="" />;
      }
      }
    }
  }
}