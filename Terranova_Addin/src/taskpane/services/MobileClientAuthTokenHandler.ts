/* eslint-disable no-undef */
import axios from "axios";
import { ApiUrls } from "../environments/ApiUrls";
import { MessageText } from "../utils/constants/MessageText";
import { LocalStorageItemNames } from "../utils/constants/LocalStorageItemNames";

export async function getAuthToken(code: string, showError: Function, closeDialogBox: Function) {
  return new Promise((resolve, reject) => {
    let requestInfo = {
      Code: code,
    };
    axios
      .post(ApiUrls.urlGetAccessTokenFromCode, requestInfo)
      .then((response) => {
        console.log(response);
        if (response.status === 200) {
          let result = response.data;
          if (result.isError === false) {
            if (localStorage.getItem(LocalStorageItemNames.REFRESH_TOKEN)) {
              localStorage.removeItem(LocalStorageItemNames.REFRESH_TOKEN);
              localStorage.setItem(LocalStorageItemNames.REFRESH_TOKEN, result.tokenInfo.refreshToken);
            } else {
              localStorage.setItem(LocalStorageItemNames.REFRESH_TOKEN, result.tokenInfo.refreshToken);
            }
            resolve(closeDialogBox());
          } else {
            reject(showError(result.errorMessage));
          }
        } else {
          if (response.data) {
            console.log(response.data);
            const { isError, errorMessage } = response.data;
            isError ? reject(showError(errorMessage)) : reject(showError(MessageText.commonErrorMessage));
          } else {
            reject(showError(MessageText.commonErrorMessage));
          }
        }
      })
      .catch((error) => {
        console.log(error);
        console.log(error.response);
        if (error.response) {
          const { isError, errorMessage } = error.response.data;
          console.log(errorMessage);
          isError ? reject(showError(errorMessage)) : reject(showError(MessageText.commonErrorMessage));
        } else {
          reject(showError(MessageText.commonErrorMessage));
        }
      });
  });
}
