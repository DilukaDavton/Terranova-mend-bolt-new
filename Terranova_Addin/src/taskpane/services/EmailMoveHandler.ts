/* eslint-disable no-undef */
import axios from "axios";
import { ApiUrls } from "../environments/ApiUrls";
import { MessageText } from "../utils/constants/MessageText";
import Payload from "../models/Payload";

export async function moveEmail(
  bootstrapToken: string,
  showError: Function,
  setEmailMoveStatus: Function,
  payload: Payload,
  isSimulation: boolean,
) {
  return new Promise<void>((resolve, reject) => {
    let params = JSON.stringify({
      MailItemId: payload.mailItemId,
      MailboxAddress: payload.mailboxAddress,
      IsSimulation: isSimulation,
      ConfigurationInfo: payload.configurationInfo,
    });
    console.log(params);
    axios
      .post(ApiUrls.urlMoveEmail, params, {
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${bootstrapToken}`,
        },
      })
      .then((response) => {
        if (response.status === 200) {
          resolve(setEmailMoveStatus());
        } else {
          if (response.data) {
            const { message } = response.data;
            message ? reject(showError(message)) : reject(showError(response.data));
          } else if (response.statusText) {
            reject(showError(response.statusText));
          } else {
            reject(showError(MessageText.commonErrorMessage));
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
            reject(showError(MessageText.commonErrorMessage));
          }
          reject(showError(error.message));
        }
      });
  });
}
