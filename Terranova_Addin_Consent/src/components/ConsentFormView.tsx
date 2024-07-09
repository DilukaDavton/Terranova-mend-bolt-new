import React, { useEffect, useState } from "react";
import "./ConsentFormView.css";
import {
  Button,
  Card,
  CardFooter,
  CardHeader,
  Subtitle1,
  Text,
  Toast,
  ToastBody,
  ToastTitle,
  Toaster,
  makeStyles,
  shorthands,
  tokens,
  useId,
  useToastController,
} from "@fluentui/react-components";
import { Status24Filled } from "@fluentui/react-icons";
import { Configuration } from "../models/Configuration";
import { CommonIdentifiers } from "../utils/constants/CommonIdentifiers";
import { MessageText } from "../utils/constants/MessageTexts";
import ConsentErrorView from "./ConsentErrorView";

interface ConsentFormViewProps {
  config?: Configuration;
}

const useStyles = makeStyles({
  main: {
    ...shorthands.gap("12px"),
    display: "flex",
    flexDirection: "column",
    flexWrap: "wrap",
  },
  card: {
    maxWidth: "100%",
    textAlign: "left",
    marginBottom: "8px",
  },
  section: {
    width: "fit-content",
  },
  title: {
    ...shorthands.margin(0, 0, "8px"),
  },
  horizontalCardImage: {
    width: "64px",
    height: "64px",
  },
  headerImage: {
    ...shorthands.borderRadius("4px"),
    maxWidth: "42px",
    maxHeight: "42px",
  },
  caption: {
    color: tokens.colorNeutralForeground3,
  },
  text: {
    ...shorthands.margin(0),
  },
});

const ConsentFormView: React.FC<ConsentFormViewProps> = ({ config }) => {
  const [ssoStepConsent, setSSOStepConsent] = useState<boolean>(false);
  const [mobileStepConsent, setMobileStepConsent] = useState<boolean>(false);
  const [ssoStepTenantId, setSSOStepTenantId] = useState<string | null>(null);
  const [mobileStepTenantId, setMobileStepTenantId] = useState<string | null>(
    null
  );
  const [consentError, setConsentError] = useState<boolean | null>(false);

  const styles = useStyles();
  const toasterId = useId("toaster");
  const { dispatchToast } = useToastController(toasterId);

  const notifyErrorToast = (title: string, error: string) => {
    dispatchToast(
      <Toast>
        <ToastTitle>{title}</ToastTitle>
        <ToastBody subtitle="Subtitle">{error}</ToastBody>
      </Toast>,
      { intent: "error" }
    );
  };

  useEffect(() => {
    const searchParams = new URLSearchParams(window.location.search);
    if (
      searchParams.get("error") != null &&
      searchParams.get("error_description") != null
    ) {
      console.error(
        searchParams.get("error") +
          " => " +
          searchParams.get("error_description")
      );
      notifyErrorToast(
        MessageText.consentFailedMessageTitle,
        MessageText.consentFailedMessageBody
      );
    } else {
      if (
        searchParams.get("state") === "step1" &&
        searchParams.get("admin_consent") === "True"
      ) {
        setSSOStepConsent(true);
        setConsentError(false);
        setSSOStepTenantId(searchParams.get("tenant"));
      }
    }
  }, []);

  useEffect(() => {
    const searchParams = new URLSearchParams(window.location.search);
    const stateVal = searchParams.get("state");

    if (stateVal != null && stateVal.split("~") != null) {
      var splitState = stateVal.split("~");
      if (splitState[0] != null && splitState[1] != null) {
        setSSOStepConsent(true);
        setSSOStepTenantId(decrypt(splitState[1], 10));
        var previousTenantID = splitState[1];
        var newTenantID = searchParams.get("tenant");
        if (newTenantID != null) {
          setMobileStepTenantId(newTenantID);
          var newTenantIdHashed = encrypt(newTenantID, 10);
          console.log(previousTenantID);
          console.log(newTenantIdHashed);
          if (previousTenantID === newTenantIdHashed) {
            setMobileStepConsent(true);
            setConsentError(false);
          } else {
            setSSOStepConsent(false);
            setMobileStepConsent(false);
            setConsentError(true);
          }
        } else {
          console.log("No tenant id in request!");
        }
      }
    }
  }, []);

  const handleStep1ButtonClick = () => {
    const redirectUri = window.location.origin;
    if (config !== undefined) {
      const step1ConsentUrl =
        `https://login.microsoftonline.com/common/adminconsent` +
        `?client_id=${config.graphAppId}` +
        `&redirect_uri=${redirectUri}` +
        "&state=step1";
      window.location.href = step1ConsentUrl;
    }
  };

  const handleStep2ButtonClick = () => {
    var hashedTenantId = encrypt(ssoStepTenantId || "", 10);
    const redirectUri = window.location.origin;
    if (config !== undefined) {
      const step2ConsentUrl =
        `https://login.microsoftonline.com/common/adminconsent` +
        `?client_id=${config.clientAppId}` +
        `&redirect_uri=${redirectUri}` +
        `&state=step2~${hashedTenantId}`;
      window.location.href = step2ConsentUrl;
    }
  };

  const encrypt = (text: string, shift: number): string => {
    return text
      .split("")
      .map((char) => {
        if (char.match(/[a-z]/i)) {
          const code = char.charCodeAt(0);
          const base =
            char === char.toLowerCase() ? "a".charCodeAt(0) : "A".charCodeAt(0);
          return String.fromCharCode(((code - base + shift) % 26) + base);
        }
        return char;
      })
      .join("");
  };

  const decrypt = (text: string, shift: number): string => {
    return encrypt(text, 26 - shift);
  };

  const renderConsentCard = (
    consentStatus: boolean,
    headerText: string,
    statusText: string,
    buttonText: string,
    isDisabled: boolean,
    onClickHandler?: () => void
  ) => {
    const iconColor = consentStatus ? "green" : "red";
    return (
      <Card className={styles.card} orientation="vertical">
        <CardHeader
          image={<Status24Filled color={iconColor} />}
          header={<Text weight="semibold">{headerText}</Text>}
          description={<Text className={styles.caption}>{statusText}</Text>}
        />
        <CardFooter className={styles.main}>
          <Button size="small" onClick={onClickHandler} disabled={isDisabled}>
            {buttonText}
          </Button>
        </CardFooter>
      </Card>
    );
  };

  return (
    <div className="centeredBox">
      <Toaster toasterId={toasterId} />
      <div>
        {!consentError ? (
          <div>
            {renderConsentCard(
              ssoStepConsent,
              CommonIdentifiers.phishSSOConsentStatusHeader,
              ssoStepConsent
                ? CommonIdentifiers.phishConsentGrantedStatusText
                : CommonIdentifiers.phishConsentNotGrantedStatusText,
              CommonIdentifiers.phishConsentGrantButtonText,
              ssoStepConsent,
              handleStep1ButtonClick
            )}
            {renderConsentCard(
              mobileStepConsent,
              CommonIdentifiers.phishMobileConsentStatusHeader,
              mobileStepConsent
                ? CommonIdentifiers.phishConsentGrantedStatusText
                : CommonIdentifiers.phishConsentNotGrantedStatusText,
              CommonIdentifiers.phishConsentGrantButtonText,
              ssoStepConsent ? mobileStepConsent : true,
              handleStep2ButtonClick
            )}
            {ssoStepConsent && mobileStepConsent && (
              <Subtitle1>
                {CommonIdentifiers.phishConsentsSuccessfulMessage}
              </Subtitle1>
            )}
          </div>
        ) : (
          <ConsentErrorView
            ssoStepTenantId={ssoStepTenantId}
            mobileStepTenantId={mobileStepTenantId}
          />
        )}
      </div>
    </div>
  );
};

export default ConsentFormView;
