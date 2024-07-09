/* eslint-disable no-undef */
import axios from "axios";
import { ApiUrls } from "../environments/ApiUrls";
import { MessageText } from "../utils/constants/MessageText";

export async function getConfigData(envId: string, locale: string, setConfigData: Function, showError: Function) {
  return new Promise<any>((resolve, reject) => {
    axios
      .get(`${ApiUrls.urlConfigData}?envId=${envId}&locale=${locale}`)
      .then((response) => {
        console.log(response);
        if (response.status === 200) {
          resolve(setConfigData(response.data));
        } else {
          if (response.data) {
            const { message } = response.data;
            message ? reject(showError(message)) : reject(showError(response.data));
          } else if (response.statusText) {
            reject(showError(response.statusText));
          } else {
            reject(
              showError(
                locale.indexOf("fr") === 0
                  ? MessageText.obtainingConfigorationErrorMessageFrench
                  : MessageText.obtainingConfigorationErrorMessageEnglish
              )
            );
          }
        }
      })
      .catch((error) => {
        console.log(error);
        console.log(error.response);
        if (error.response) {
          const { data, statusText } = error.response;
          if (data) {
            data.message ? reject(showError(data.message)) : reject(showError(data));
          } else if (statusText) {
            reject(showError(statusText));
          } else {
            reject(
              showError(
                locale.indexOf("fr") === 0
                  ? MessageText.obtainingConfigorationErrorMessageFrench
                  : MessageText.obtainingConfigorationErrorMessageEnglish
              )
            );
          }
        } else {
          reject(showError(error.message));
        }
      });
  });
}
