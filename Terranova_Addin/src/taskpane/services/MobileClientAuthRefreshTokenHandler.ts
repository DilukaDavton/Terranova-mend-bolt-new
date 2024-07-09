/* eslint-disable no-undef */
import axios from "axios";
import { ApiUrls } from "../environments/ApiUrls";
import { LocalStorageItemNames } from "../utils/constants/LocalStorageItemNames";

export async function getAccessToken(
  refreshToken: string,
  setAccessToken: Function,
  setRefreshTokenAsInvalid: Function
) {
  return new Promise((resolve, reject) => {
    let requestInfo = {
      RefreshToken: refreshToken,
    };
    axios
      .post(ApiUrls.urlGetAccessTokenFromRefreshToken, requestInfo)
      .then((response) => {
        console.log(response);
        if (response.status === 200) {
          let result = response.data;
          if (result.isError === false) {
            localStorage.setItem(LocalStorageItemNames.REFRESH_TOKEN,result.tokenInfo.refreshToken);
            resolve(setAccessToken(result.tokenInfo.accessToken));
          } else {
            reject(setRefreshTokenAsInvalid());
          }
        } else {
          reject(setRefreshTokenAsInvalid());
        }
      })
      .catch((error) => {
        console.log(error);
        console.log(error.response);
        reject(setRefreshTokenAsInvalid());
      });
  });
}
