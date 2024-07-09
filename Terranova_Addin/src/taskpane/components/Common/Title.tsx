import * as React from "react";

export interface TitleProps {
  caption: string;
}

export default class Title extends React.Component<TitleProps> {
  render() {
    const { caption } = this.props;
    return <h4 className="ms-fontSize-xxl ms-fontWeight-light ms-fontColor-neutralPrimary">{caption}</h4>;
  }
}
