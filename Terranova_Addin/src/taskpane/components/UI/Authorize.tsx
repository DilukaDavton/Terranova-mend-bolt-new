/* eslint-disable no-undef */
import * as React from "react";
import Progress from "../Common/Progress";
import Title from "../Common/Title";
import Button from "../Common/Button";
import Configuration from "../../models/Configuration";

export interface AuthorizeProps {
  onClickAuthorizeHandler: VoidFunction;
  configData: Configuration;
  authError: String;
}

export interface AuthorizeState {
  isProcessing: boolean;
}

export default class Authorize extends React.Component<AuthorizeProps, AuthorizeState> {
  constructor(props, context) {
    super(props, context);
    this.state = {
      isProcessing: false,
    };
  }

  componentDidMount() {
    if (this.props.configData) {
      // check whether user authorize screen not needed to display and there isn't any auth errors
      if (!this.props.configData.userAuthorizationScreenDisplay && !this.props.authError) {
        // directly call the auth pop-up without button click
        this.props.onClickAuthorizeHandler();
      }
    }
  }

  onClickBtnHandler = () => {
    this.setState({ isProcessing: true });
    this.props.onClickAuthorizeHandler();
  };

  render() {
    if (this.props.configData.userAuthorizationScreenDisplay || this.props.authError) {
      if (this.state.isProcessing) {
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
