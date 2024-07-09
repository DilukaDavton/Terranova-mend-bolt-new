import { UrlParams } from "../../models/UrlParams";
import {UrlParamNames} from "../constants/UrlParamNames";

export function getUrlParamsValues(){
    let urlParams = location.search;
    let paramsArray = urlParams.substring(1).split("&");
    let urlParamsInfo: UrlParams={
        environmentId:"",
        isIntuneConfigured:false,
        code:"",
        state:""
    }
    if (urlParams.includes(UrlParamNames.ENVIRONMENT_ID)) {
      urlParamsInfo.environmentId =  decodeURIComponent(paramsArray.find((x) => x.startsWith(UrlParamNames.ENVIRONMENT_ID))).split("=")[1];
    }
    if (urlParams.includes(UrlParamNames.INTUNE_CONFIGURED)) {
        let queryParamVal = decodeURIComponent(paramsArray.find((x) => x.startsWith(UrlParamNames.INTUNE_CONFIGURED))).split("=")[1]
        urlParamsInfo.isIntuneConfigured = queryParamVal==='true'? true: false;
      }
    return urlParamsInfo;
}