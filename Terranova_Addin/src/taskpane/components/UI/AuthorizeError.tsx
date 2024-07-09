import * as React from "react";
import MessageInfoBar from "../Common/MessageInfoBar";
import Authorize from "./Authorize";
import IntuneConfiguredAndroidAuthorize from "./IntuneConfiguredAndroidAuthorize";
import IntuneConfiguredIOSAuthorize from "./IntuneConfiguredIOSAuthorize";
import { MessageTypes } from "../../utils/constants/MessageTypes";
import Configuration from "../../models/Configuration";
import { OutlookHostNames } from "../../utils/constants/OutlookHostNames";
import { LocalStorageItemNames } from "../../utils/constants/LocalStorageItemNames";

export interface AuthorizeErrorProps {
  isIntuneConfigured: boolean;
  authError: string;
  hostName: string;
  configData: Configuration;
  clearErrorForReAuthenticate: VoidFunction;
  setAuthorizingStatus: VoidFunction;
  getMSAuthUrl: VoidFunction;
  openDialogApiForAndroidAuth: VoidFunction;
  onClickForDialogAuthorization: VoidFunction;
}

export interface AuthorizeErrorState {}

export default class AuthorizeError extends React.Component<AuthorizeErrorProps, AuthorizeErrorState> {
  constructor(props, context) {
    super(props, context);
  }

  componentDidMount() {
    localStorage.removeItem(LocalStorageItemNames.REFRESH_TOKEN);
  }

  render() {
    if (this.props.isIntuneConfigured && this.props.hostName === OutlookHostNames.IOS) {
      return (
        <>
          <MessageInfoBar message={this.props.authError} messageType={MessageTypes.error} />
          <div style={{ marginTop: "25px" }}></div>
          <IntuneConfiguredIOSAuthorize
            setAuthorizingStatus={this.props.setAuthorizingStatus}
            getMSAuthUrl={this.props.getMSAuthUrl}
            configData={this.props.configData}
            clearErrorForReAuthenticate={this.props.clearErrorForReAuthenticate}
            authError={this.props.authError}
          ></IntuneConfiguredIOSAuthorize>
        </>
      );
    } else if (this.props.isIntuneConfigured && this.props.hostName === OutlookHostNames.ANDROID) {
      return (
        <>
          <MessageInfoBar message={this.props.authError} messageType={MessageTypes.error} />
          <div style={{ marginTop: "25px" }}></div>
          <IntuneConfiguredAndroidAuthorize
            openDialogAPIForAndroidAuth={this.props.openDialogApiForAndroidAuth}
            configData={this.props.configData}
            authError={this.props.authError}
          ></IntuneConfiguredAndroidAuthorize>
        </>
      );
    } else {
      return (
        <>
          <MessageInfoBar message={this.props.authError} messageType={MessageTypes.error} />
          <div style={{ marginTop: "25px" }}></div>
          <Authorize
            onClickAuthorizeHandler={this.props.onClickForDialogAuthorization}
            configData={this.props.configData}
            authError={this.props.authError}
          />
        </>
      );
    }
  }
}
