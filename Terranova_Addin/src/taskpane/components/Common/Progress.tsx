import * as React from "react";
import { Spinner } from "@fluentui/react";

export interface ProgressProps {
  message: string;
}

export default class Progress extends React.Component<ProgressProps> {
  render() {
    const { message } = this.props;

    return (
      <section className="ms-welcome__header ms-u-fadeIn500">
        <Spinner label={message} />
      </section>
    );
  }
}
