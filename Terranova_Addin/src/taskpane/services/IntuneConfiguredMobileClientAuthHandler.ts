import axios from "axios";
import { ApiUrls } from "../environments/ApiUrls";

export async function getMSAuthUrl(addinGuid: string, showError: Function){
    return new Promise<string>((resolve, reject) =>{
      axios.get(`${ApiUrls.urlGetMSRedirectUrl}?tokenRetrievalGuid=${addinGuid}&isIntuneConfigured=${true}`).then((response)=>{
        resolve(response.data);
      }).catch((error)=>{
        reject(showError(error.message));
      });
    });
  }
  
  export async function getRefreshTokenForGuid(tokenRetrievalGuid: string, showError: Function){
    return new Promise<string>((resolve, reject) =>{
      axios.get(`${ApiUrls.urlGetRefreshTokenForGuid}?tokenRetrievalGuid=${tokenRetrievalGuid}`).then((response)=>{
        resolve(response.data.refreshToken);
      }).catch((error)=>{
        reject(showError(error.message));
      });
    });
  }