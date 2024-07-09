/* eslint-disable no-undef */
import axios from "axios";
import { ApiUrls } from "../environments/ApiUrls";
import { MessageText } from "../utils/constants/MessageText";

export async function authenticate(
  bootstrapToken: string,
  handleAuthError: Function,
  updateUserVerification: Function,
) {
  return new Promise((resolve, reject) => {
    axios
      .post(ApiUrls.urlAuthUser, null, {
        headers: {
          Authorization: `Bearer ${bootstrapToken}`,
        },
      })
      .then((response) => {
        console.log(response);
        if (response.status === 200) {
          resolve(updateUserVerification());
        } else {
          if (response.data) {
            const { message } = response.data;
            message ? reject(handleAuthError(message)) : reject(handleAuthError(response.data));
          } else if (response.statusText) {
            reject(handleAuthError(response.statusText));
          } else {
            reject(handleAuthError(MessageText.commonErrorMessage));
          }
        }
      })
      .catch((ex) => {
        console.log(ex);
        console.log(ex.response);

        if (ex.response) {
          const { data, statusText } = ex.response;
          if (data) {
            if (data.Classification) {
              if (data.Classification === 3) {
                //invalid client
                reject(handleAuthError("", true));
              }
              else {
                data.Message ? reject(handleAuthError(data.Message)) : reject(handleAuthError(data));
              }
            }
            else {
              data.message ? reject(handleAuthError(data.message)) : reject(handleAuthError(data));
            }
            //reject(showError(JSON.stringify(data)));
          } else if (statusText) {
            reject(handleAuthError(statusText));
          } else {
            reject(handleAuthError(MessageText.commonErrorMessage));
          }
        } else {
          reject(handleAuthError(ex.message));
        }
      });
  });
}
