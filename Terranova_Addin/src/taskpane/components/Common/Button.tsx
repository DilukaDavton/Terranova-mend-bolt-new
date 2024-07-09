import * as React from "react";
import { ButtonType, PrimaryButton } from "@fluentui/react";

export interface ButtonProps {
  caption: string;
  textColor: string;
  backgroundColor: string;
  onClickHandler: VoidFunction;
  href: string;
}

export default class Button extends React.Component<ButtonProps> {
  render() {
    const { caption, textColor, backgroundColor, onClickHandler, href } = this.props;
    const styles = {
      rootHovered: {
        backgroundColor: backgroundColor,
        border: `1px solid ${backgroundColor}`,
        color: textColor,
        filter: "brightness(85%)",
      },
      root: [
        {
          fontSize: "16px",
          border: `1px solid ${backgroundColor}`,
          borderRadius: "10px",
          padding: "0px 30px",
          height: "40px",
          width: "100%",
          backgroundColor: backgroundColor,
          color: textColor,
          "&:active": {
            backgroundColor: backgroundColor,
            border: `1px solid ${backgroundColor}`,
            color: textColor,
            filter: "brightness(100%)",
          },
          "&:focus": {
            backgroundColor: backgroundColor,
            border: `1px solid ${backgroundColor}`,
            color: textColor,
            filter: "brightness(100%)",
          },
        },
      ],
    };
    return (
      <PrimaryButton buttonType={ButtonType.normal} onClick={onClickHandler} styles={styles} href={href}>
        {caption}
      </PrimaryButton>
    );
  }
}
