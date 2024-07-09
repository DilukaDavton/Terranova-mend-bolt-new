/* eslint-disable no-undef */
import * as React from "react";
import Button from "../Common/Button";
import Title from "../Common/Title";

export interface MoveProps {
  feedbackMessage: string;
  moveButtonCaption: string;
  moveButtonBackgroundColor: string;
  moveButtonTextColor: string;
  isEmailInJunkOrDeletedItemsFolder: boolean;
  onMoveClickHandler: VoidFunction;
}

export default class Move extends React.Component<MoveProps> {
  render() {
    const {
      feedbackMessage,
      moveButtonCaption,
      moveButtonBackgroundColor,
      moveButtonTextColor,
      isEmailInJunkOrDeletedItemsFolder,
      onMoveClickHandler,
    } = this.props;
    if (isEmailInJunkOrDeletedItemsFolder) {
      return (
        <div className="ms-Grid ms-welcome__action">
          <div className="ms-Grid">
            <Title caption={feedbackMessage} />
          </div>
        </div>
      );
    } else {
      return (
        <div className="ms-Grid ms-welcome__action">
          <div className="ms-Grid">
            <Title caption={feedbackMessage} />
            <Button
              caption={moveButtonCaption}
              textColor={moveButtonTextColor}
              backgroundColor={moveButtonBackgroundColor}
              onClickHandler={onMoveClickHandler}
              href="#"
            />
          </div>
        </div>
      );
    }
  }
}
