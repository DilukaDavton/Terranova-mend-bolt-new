import React from "react";
import "./ConsentFormView.css";
import { Button, Subtitle1, Text } from "@fluentui/react-components";

interface ConsentErrorViewProps {
  ssoStepTenantId: string | null;
  mobileStepTenantId: string | null;
}

const ConsentErrorView: React.FC<ConsentErrorViewProps> = ({
  ssoStepTenantId,
  mobileStepTenantId,
}) => {
  const handleRetryButtonClick = () => {
    window.location.href = window.location.origin;
  };

  return (
    <div>
      <br></br>
      <Subtitle1>Tenant Id Mismatch</Subtitle1>
      <br></br>
      <br></br>
      <Text weight="bold">
        We're sorry, but there seems to be an issue with your request. The
        tenant Ids [{ssoStepTenantId}, {mobileStepTenantId}] for the two consent
        URLs you provided do not match. For security reasons, we cannot proceed
        with the operation.
      </Text>
      <br></br>
      <br></br>
      <Text weight="bold">
        We advise exercising caution while interacting with your accounts and
        refrain from sharing sensitive information with unauthorized
        individuals. Thank you for your understanding.
      </Text>
      <br></br>
      <br></br>
      <Button size="small" onClick={handleRetryButtonClick}>
        Start Over
      </Button>
    </div>
  );
};

export default ConsentErrorView;
