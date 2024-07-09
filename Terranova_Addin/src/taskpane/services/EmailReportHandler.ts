/* eslint-disable no-undef */
import axios from "axios";
import { ApiUrls } from "../environments/ApiUrls";
import { MessageText } from "../utils/constants/MessageText";
import Payload from "../models/Payload";

export async function reportEmail(
  bootstrapToken: string,
  setReportStatus: Function,
  showError: Function,
  payload: Payload
) {
  return new Promise<void>((resolve, reject) => {
    let params = JSON.stringify({
      MailItemId: payload.mailItemId,
      InputFromCommentBox: payload.feedbackMessage,
      EnvironmentId: payload.environmentId,
      MailboxAddress: payload.mailboxAddress,
      SimulationInfo: payload.simulationInfo,
      EmailTypeSelectionInfo: payload.emailType,
      ConfigurationInfo: payload.configurationInfo
    });
    axios
      .post(ApiUrls.urlReportEmail, params, {
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${bootstrapToken}`,
        },
      })
      .then((response) => {
        console.log(response);
        if (response.status === 200) {
          resolve(setReportStatus(response.data));
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
        } else {
          reject(showError(error.message));
        }
      });
  });
}

export async function getPreviouslyReportedStatus(
  bootstrapToken: string,
  setPreviouslyReportedStatus: Function,
  showError: Function,
  payload: Payload
) {
  return new Promise<void>((resolve, reject) => {
    let params = JSON.stringify({
      MailItemId: payload.mailItemId,
      MailboxAddress: payload.mailboxAddress,
    });
    axios
      .post(ApiUrls.urlGetPreviouslyReportedStatus, params, {
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${bootstrapToken}`,
        },
      })
      .then((response) => {
        console.log(response);
        if (response.status === 200) {
          resolve(setPreviouslyReportedStatus(response.data));
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
        } else {
          reject(showError(error.message));
        }
      });
  });
}
