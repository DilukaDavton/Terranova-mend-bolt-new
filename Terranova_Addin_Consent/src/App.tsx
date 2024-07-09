import { useEffect, useState } from "react";
import "./App.css";
import ConsentFormView from "./components/ConsentFormView";
import { Subtitle2 } from "@fluentui/react-components";
import appLogo from "./assets/terranova_logo.png";
import { Configuration } from "./models/Configuration";
import LoadingSpinnerView from "./components/LoadingSpinnerView";
import { getConfigData } from "./services/ConfigurationHandler";
import ConfigErrorMessageView from "./components/ConfigErrorMessageView";

function App() {
  const [isConfigured, setIsConfigured] = useState<boolean | null>(null);
  const [isConfigError, setIsConfigError] = useState<string>("");
  const [configErrorMessage, setConfigErrorMessage] = useState<string>("");
  const [config, setConfig] = useState<Configuration>();

  useEffect(() => {
    const getConfig = async () => {
      await getConfigData(
        setConfig,
        setIsConfigured,
        setIsConfigError,
        setConfigErrorMessage
      );
    };
    getConfig();
  }, []);

  return (
    <div className="App">
      <div className="App-header">
        <img
          src={appLogo}
          alt="FORTRA Terranova Security Logo"
          className="image-header"
        />
        <Subtitle2 className="header-subtitle">
          Phish Submitter Admin Consent Wizard
        </Subtitle2>
      </div>
      <center>
        {isConfigured ? (
          isConfigError ? (
            <ConfigErrorMessageView message={configErrorMessage} />
          ) : (
            <ConsentFormView config={config} />
          )
        ) : (
          <LoadingSpinnerView />
        )}
      </center>
    </div>
  );
}

export default App;
