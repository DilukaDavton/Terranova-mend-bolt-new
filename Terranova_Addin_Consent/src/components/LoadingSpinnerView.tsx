import React from "react";
import { Spinner } from "@fluentui/react-components";
import "./LoadingSpinnerView.css";
import { MessageText } from "../utils/constants/MessageTexts";

const LoadingSpinnerView: React.FC = () => {
  return (
    <div className="loading-spinner-container">
      <Spinner
        size="huge"
        labelPosition="below"
        label={MessageText.wizardLoadingMessage}
      />
    </div>
  );
};

export default LoadingSpinnerView;
