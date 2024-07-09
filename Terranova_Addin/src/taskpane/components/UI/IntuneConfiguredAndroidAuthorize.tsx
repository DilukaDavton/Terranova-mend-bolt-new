/* eslint-disable no-undef */
import * as React from "react";
import Progress from "../Common/Progress";
import Title from "../Common/Title";
import Button from "../Common/Button";
import Configuration from "../../models/Configuration";

export interface IntuneConfiguredAndroidAuthorizeProps {
  openDialogAPIForAndroidAuth: VoidFunction;
  configData: Configuration;
  authError: String;
}

export interface IntuneConfiguredAndroidAuthorizeState {
  isLoading: boolean;
}

export default class IntuneConfiguredAndroidAuthorize extends React.Component<
  IntuneConfiguredAndroidAuthorizeProps,
  IntuneConfiguredAndroidAuthorizeState
> {
  constructor(props, context) {
    super(props, context);
    this.state = {
      isLoading: false,
    };
  }

  componentDidMount() {
    if (this.props.configData) {
      // check whether user authorize screen not needed to display and there isn't any auth errors
      if (!this.props.configData.userAuthorizationScreenDisplay && !this.props.authError) {
        this.props.openDialogAPIForAndroidAuth();
      }
    }
  }

  onClickBtnHandler = () => {
    this.props.openDialogAPIForAndroidAuth();
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
                href="#"
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
