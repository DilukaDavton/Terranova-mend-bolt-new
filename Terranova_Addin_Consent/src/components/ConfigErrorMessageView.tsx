import React from "react";
import "./ConfigErrorMessageView.css"; // Create a CSS file to style the message box (optional)
import { MessageText } from "../utils/constants/MessageTexts";

interface ConfigErrorMessageViewProps {
  message?: string;
}

const ConfigErrorMessageView: React.FC<ConfigErrorMessageViewProps> = ({
  message,
}) => {
  const errorMessage = message || MessageText.configFetchFailedErrorCommon;

  return (
    <div className="error-message-container">
      <h2>{MessageText.configFetchFailedErrorHeader}</h2>
      <p>{errorMessage}</p>
    </div>
  );
};

export default ConfigErrorMessageView;
