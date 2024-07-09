/* eslint-disable no-undef */
import * as React from "react";
import Configuration from "../../models/Configuration";
import Button from "../Common/Button";
import Title from "../Common/Title";

export interface ConfirmationProps {
  configuration: Configuration;
  onNextClickHandler: VoidFunction;
}

export default class Confirmation extends React.Component<ConfirmationProps> {
  render() {
    const { configuration, onNextClickHandler } = this.props;
    return (
      <div className="ms-Grid ms-welcome__action">
        <div className="ms-Grid">
          <Title caption={configuration.confirmationPromptMessageCaption} />
          <Button
            caption={configuration.nextConfirmationMessageButtonText}
            textColor={configuration.nextConfirmationMessageButtonForegroundColor}
            backgroundColor={configuration.nextConfirmationMessageButtonBackgroundColor}
            onClickHandler={onNextClickHandler}
            href="#"
          />
        </div>
      </div>
    );
  }
}
