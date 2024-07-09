import * as React from "react";
import { MessageBar, MessageBarType } from "@fluentui/react";
import { MessageTypes } from "../../utils/constants/MessageTypes";

export interface MessageInfoBarProps {
  message: string;
  messageType: string;
}

export default class MessageInfoBar extends React.Component<MessageInfoBarProps> {
  render() {
    const { message, messageType } = this.props;
    return (
      <div className="ms-welcome">
        <MessageBar
          isMultiline={true}
          messageBarType={
            messageType == MessageTypes.success
              ? MessageBarType.success
              : messageType == MessageTypes.error
              ? MessageBarType.error
              : MessageBarType.info
          }
          dismissButtonAriaLabel="Close"
          truncated={true}
          overflowButtonAriaLabel="See more"
        >
          {message}
        </MessageBar>
      </div>
    );
  }
}
