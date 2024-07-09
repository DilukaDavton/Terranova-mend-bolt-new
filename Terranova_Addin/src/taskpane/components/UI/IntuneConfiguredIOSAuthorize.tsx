/* eslint-disable no-undef */
import * as React from "react";
import Progress from "../Common/Progress";
import Title from "../Common/Title";
import Button from "../Common/Button";
import Configuration from "../../models/Configuration";

export interface IntuneConfiguredIOSAuthorizeProps {
  setAuthorizingStatus: VoidFunction;
  getMSAuthUrl: Function;
  configData: Configuration;
  clearErrorForReAuthenticate: VoidFunction;
  authError: String;
}

export interface IntuneConfiguredIOSAuthorizeState {
  isLoading: boolean;
  msAuthUrl: string;
}

export default class IntuneConfiguredIOSAuthorize extends React.Component<
  IntuneConfiguredIOSAuthorizeProps,
  IntuneConfiguredIOSAuthorizeState
> {
  constructor(props, context) {
    super(props, context);
    this.state = {
      isLoading: true,
      msAuthUrl: "",
    };
  }

  async componentDidMount() {
    //get the ms auth url if device is ios
    let authUrl = await this.props.getMSAuthUrl();
    this.setState({ msAuthUrl: authUrl, isLoading: false });

    if (this.props.configData) {
      // check whether user authorize screen not needed to display and there isn't any auth errors
      if (!this.props.configData.userAuthorizationScreenDisplay && !this.props.authError) {
        await this.props.setAuthorizingStatus();
        window.open(this.state.msAuthUrl);
      }
    }
  }

  onClickBtnHandler = () => {
    this.props.clearErrorForReAuthenticate();
    this.props.setAuthorizingStatus();
    return true;
  };

  render() {
    if (this.props.configData.userAuthorizationScreenDisplay || this.props.authError) {
      if (this.state.isLoading) {
        return <Progress message={this.props.configData.loadingLabelText} />;
      } else {
        return (
          <div className="ms-Grid ms-welcome__action">
            <div className="ms-Grid">
              <Title caption={this.props.configData.userAuthorizationDescriptionText} />
            </div>
            <div className="ms-Grid">
              <Button
                caption={this.props.configData.userAuthorizationButtonText}
                textColor={this.props.configData.authorizeButtonForegroundColor}
                backgroundColor={this.props.configData.authorizeButtonBackgroundColor}
                onClickHandler={this.onClickBtnHandler}
                href={this.state.msAuthUrl}
              />
            </div>
          </div>
        );
      }
    } else {
      return <Progress message={this.props.configData.loadingLabelText} />;
    }
  }
}
