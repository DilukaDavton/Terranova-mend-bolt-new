import axios, { AxiosResponse } from "axios";

export async function getConfigData(
  setConfig: Function,
  setIsConfigured: Function,
  setIsConfigError: Function,
  setConfigErrorMessage: Function
): Promise<void> {
  const appUrlOrigin = window.location.origin;
  const configEndPointUrl = "api/configuration/consentconfiguration";

  try {
    const response: AxiosResponse<any> = await axios.get(
      `${appUrlOrigin}/${configEndPointUrl}`
    );

    if (response.status === 200) {
      setConfig(response.data);
      setIsConfigError(false);
    } else {
      setConfigErrorMessage(response.data);
      setIsConfigError(true);
    }
  } catch (error) {
    setIsConfigError(true);
  }
  setIsConfigured(true);
}
