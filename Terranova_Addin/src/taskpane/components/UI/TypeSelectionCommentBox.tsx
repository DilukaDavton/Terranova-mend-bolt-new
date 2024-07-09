/* eslint-disable no-undef */
import * as React from "react";
import { TextField, ChoiceGroup, IChoiceGroupOption } from "@fluentui/react";
import Title from "../Common/Title";
import Progress from "../Common/Progress";
import Button from "../Common/Button";
import TypeSelection from "../../models/TypeSelection";
import Configuration from "../../models/Configuration";

export interface TypeSelectionCommentBoxProps {
  configuration: Configuration;
  getTypeSelectionAndComment: Function;
}

export interface TypeSelectionCommentBoxState {
  commentBoxInput: string;
  userTypeSelection: TypeSelection;
}

export default class TypeSelectionCommentBox extends React.Component<
  TypeSelectionCommentBoxProps,
  TypeSelectionCommentBoxState
> {
  state = {
    commentBoxInput: "",
    userTypeSelection: null,
  };

  onTextBoxChange = (e: any) => {
    console.log(e.target.value);
    this.setState({
      commentBoxInput: e.target.value,
    });
  };

  onTypeSlectionHandler = (ev: React.FormEvent<HTMLInputElement>, option: any) => {
    console.log(option);
    console.log(ev);
    this.setState({
      userTypeSelection: {
        key: option.key,
        value: option.text,
      },
    });
  };

  generateOptions = (typeSelectionButtons: any): IChoiceGroupOption[] => {
    return typeSelectionButtons.map((caption: { key: number; value: string }) => {
      return {
        key: caption.key,
        text: caption.value,
        checked: false,
      };
    });
  };

  onSubmitTypeSelectionComment = () => {
    console.log("Submit All...");
    this.props.getTypeSelectionAndComment(this.state.userTypeSelection, this.state.commentBoxInput);
  };

  render() {
    const {
      promptUserForTypeSelection,
      typeSelectionPromptMessageCaption,
      typeSelectionButtonCaptions,
      commentBoxEnabled,
      commentBoxCaption,
      nextTypeAndCommentButtonText,
      nextTypeAndCommentButtonBackgroundColor,
      nextTypeAndCommentButtonForegroundColor,
      loadingLabelText,
    } = this.props.configuration;

    const styles = {
      marginTop: "20px",
      marginBottom: "20px",
    };

    if (promptUserForTypeSelection && commentBoxEnabled) {
      return (
        <div className="ms-Grid ms-welcome__action">
          <div className="ms-Grid">
            <Title caption={typeSelectionPromptMessageCaption} />
            <ChoiceGroup
              options={this.generateOptions(typeSelectionButtonCaptions)}
              onChange={this.onTypeSlectionHandler}
            />
          </div>
          <div className="ms-Grid">
            <Title caption={commentBoxCaption} />
            <TextField multiline autoAdjustHeight value={this.state.commentBoxInput} onChange={this.onTextBoxChange} />
          </div>
          <div className="ms-Grid" style={styles}>
            <Button
              caption={nextTypeAndCommentButtonText}
              textColor={nextTypeAndCommentButtonForegroundColor}
              backgroundColor={nextTypeAndCommentButtonBackgroundColor}
              onClickHandler={this.onSubmitTypeSelectionComment}
              href="#"
            />
          </div>
        </div>
      );
    }

    if (!commentBoxEnabled && promptUserForTypeSelection) {
      return (
        <div className="ms-Grid ms-welcome__action">
          <div className="ms-Grid">
            <Title caption={typeSelectionPromptMessageCaption} />
            <ChoiceGroup
              options={this.generateOptions(typeSelectionButtonCaptions)}
              onChange={this.onTypeSlectionHandler}
            />
          </div>
          <div className="ms-Grid" style={styles}>
            <Button
              caption={nextTypeAndCommentButtonText}
              textColor={nextTypeAndCommentButtonForegroundColor}
              backgroundColor={nextTypeAndCommentButtonBackgroundColor}
              onClickHandler={this.onSubmitTypeSelectionComment}
              href="#"
            />
          </div>
        </div>
      );
    }

    if (commentBoxEnabled && !promptUserForTypeSelection) {
      return (
        <div className="ms-Grid ms-welcome__action">
          <div className="ms-Grid">
            <Title caption={commentBoxCaption} />
            <TextField multiline autoAdjustHeight value={this.state.commentBoxInput} onChange={this.onTextBoxChange} />
            <div className="ms-welcome__action" style={styles}>
              <Button
                caption={nextTypeAndCommentButtonText}
                textColor={nextTypeAndCommentButtonForegroundColor}
                backgroundColor={nextTypeAndCommentButtonBackgroundColor}
                onClickHandler={this.onSubmitTypeSelectionComment}
                href="#"
              />
            </div>
          </div>
        </div>
      );
    }

    if (!promptUserForTypeSelection && !commentBoxEnabled) {
      //this.onSubmitTypeSelectionComment();
    }

    return <Progress message={loadingLabelText} />;
  }
}
