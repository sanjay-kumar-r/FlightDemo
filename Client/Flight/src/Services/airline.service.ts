import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { APIRequestType, HeaderInfo } from 'src/Models/HeaderInfo';
import { Airlines, DiscountTags, AirlineDiscountTagMappingDetails,
   RemapAirlineDiscountTagsDetails, 
   AirlineSchedules,
   AirlineScheduleTracker} from 'src/Models/Airlines';
import { ApiExecutorService } from './api-executor.service';

@Injectable({
  providedIn: 'root'
})
export class AirlineService {

  baseUrl:string;

  getAirlinesUrl:string;
  getAirlinesByFilterconditionUrl:string;
  addAirlineUrl:string;
  updateAirlineUrl:string;
  activateDeactivateAirlineUrl:string;
  deleteAirlineUrl:string;

  getDiscountTagsUrl:string;
  getDiscountTagsByFilterconditionUrl:string;
  addDiscountTagUrl:string;
  updateDiscountTagUrl:string;
  activateDeactivateDiscountTagUrl:string;
  deleteDiscountTagUrl:string;

  getAirlineDiscountTagsMappingUrl:string;
  mapAirlinesDiscountTagsUrl:string;
  remapAirlinesDiscountTagsUrl:string;

  getSchedulesUrl:string;
  getAirlineSchedulesByAirlineIdUrl:string;
  getAirlineSchedulesByFilterConditionUrl:string;
  addScheduleUrl:string;
  reSchedulesAirlinesByRangeUrl:string;
  deleteAirlineScheduleUrl:string;
  deleteAirlineScheduleByIdsUrl:string;
  deleteAirlineScheduleByAirlineIdsUrl:string;

  getAvailableAirlinesUrl:string;

  getTokenUrl:string;
  refreshTokenUrl :string;

  apiExecutor:ApiExecutorService;

  constructor(apiExecutor:ApiExecutorService) {
    this.baseUrl = "http://localhost:9050";

    this.getAirlinesUrl = this.baseUrl.trim() + '/' + "api/v1.0/Airlines/";
    this.getAirlinesByFilterconditionUrl = this.baseUrl.trim() + '/' + "api/v1.0/Airlines/GetAirlinesByFiltercondition";
    this.addAirlineUrl = this.baseUrl.trim() + '/' + "api/v1.0/Airlines/Add";
    this.updateAirlineUrl = this.baseUrl.trim() + '/' + "api/v1.0/Airlines/Update";
    this.activateDeactivateAirlineUrl = this.baseUrl.trim() + '/' + "api/v1.0/Airlines/ActivateDeactivateAirline";
    this.deleteAirlineUrl = this.baseUrl.trim() + '/' + "api/v1.0/Airlines/Delete";

    this.getDiscountTagsUrl = this.baseUrl.trim() + '/' + "api/v1.0/DiscountTags/";
    this.getDiscountTagsByFilterconditionUrl = this.baseUrl.trim() + '/' + "api/v1.0/DiscountTags/GetDiscountTagsByFiltercondition";
    this.addDiscountTagUrl = this.baseUrl.trim() + '/' + "api/v1.0/DiscountTags/Add";
    this.updateDiscountTagUrl = this.baseUrl.trim() + '/' + "api/v1.0/DiscountTags/Update";
    this.activateDeactivateDiscountTagUrl = this.baseUrl.trim() + '/' + "api/v1.0/DiscountTags/ActivateDeactivateDiscountTag";
    this.deleteDiscountTagUrl = this.baseUrl.trim() + '/' + "api/v1.0/DiscountTags/Delete";

    this.getAirlineDiscountTagsMappingUrl = this.baseUrl.trim() + '/' + "api/v1.0/Airlines/GetAirlineDiscountTagsMapping";
    this.mapAirlinesDiscountTagsUrl = this.baseUrl.trim() + '/' + "api/v1.0/Airlines/MapAirlinesDiscountTags";
    this.remapAirlinesDiscountTagsUrl = this.baseUrl.trim() + '/' + "api/v1.0/Airlines/RemapAirlinesDiscountTags";

    this.getSchedulesUrl = this.baseUrl.trim() + '/' + "api/v1.0/AirlineSchedules/";
    this.getAirlineSchedulesByAirlineIdUrl = this.baseUrl.trim() + '/' + "api/v1.0/AirlineSchedules/GetAirlineSchedulesByAirlineId";
    this.getAirlineSchedulesByFilterConditionUrl = this.baseUrl.trim() + '/' + "api/v1.0/AirlineSchedules/GetAirlineSchedulesByFilterCondition";
    this.addScheduleUrl = this.baseUrl.trim() + '/' + "api/v1.0/AirlineSchedules/Add";
    this.reSchedulesAirlinesByRangeUrl = this.baseUrl.trim() + '/' + "api/v1.0/AirlineSchedules/ReSchedulesAirlinesByRange";
    this.deleteAirlineScheduleUrl = this.baseUrl.trim() + '/' + "api/v1.0/AirlineSchedules/Delete";
    this.deleteAirlineScheduleByIdsUrl = this.baseUrl.trim() + '/' + "api/v1.0/AirlineSchedules/DeleteByScheduleIds";
    this.deleteAirlineScheduleByAirlineIdsUrl = this.baseUrl.trim() + '/' + "api/v1.0/AirlineSchedules/DeleteByAirlineIds";
    
    this.getAvailableAirlinesUrl = this.baseUrl.trim() + '/' + "api/v1.0/AirlineScheduleTracker/GetAvailableAirlines";

    this.getTokenUrl = "api/AuthTokens/GetAuthToken";
    this.refreshTokenUrl = "api/AuthTokens/RefreshToken";

    this.apiExecutor = apiExecutor;
  }
  // Airlines
  getAirlines(airlines:Airlines|null, headerInfo:HeaderInfo) : Observable<any>
  {
    if((airlines ?? null) == null)
    { 
      return this.apiExecutor.CallAPI(APIRequestType.Get, this.getAirlinesUrl, headerInfo ,
        null, true);
    }
    else
    {
      return this.apiExecutor.CallAPI(APIRequestType.Post, this.getAirlinesByFilterconditionUrl, headerInfo ,
        airlines, true);
    }
  }

  addAirlines(airlines:Airlines, headerInfo:HeaderInfo) : Observable<any>
  {
    return this.apiExecutor.CallAPI(APIRequestType.Post, this.addAirlineUrl, headerInfo ,
      airlines, true);
  }

  updateAirline(airlines:Airlines, headerInfo:HeaderInfo) : Observable<any>
  {
    return this.apiExecutor.CallAPI(APIRequestType.Post, this.updateAirlineUrl, headerInfo ,
      airlines, true);
  }

  activateDeactivateAirline(obj:any, headerInfo:HeaderInfo) : Observable<any>
  {
    return this.apiExecutor.CallAPI(APIRequestType.Post, this.activateDeactivateAirlineUrl, headerInfo ,
      obj, true);
  }

  deleteAirline(id:number, headerInfo:HeaderInfo) : Observable<any>
  {
    return this.apiExecutor.CallAPI(APIRequestType.Post, this.deleteAirlineUrl, headerInfo ,
      id, true);
  }
  // DiscountTags
  getDiscountTags(discountTags:DiscountTags|null, headerInfo:HeaderInfo) : Observable<any>
  {
    if((discountTags ?? null) == null)
    { 
      return this.apiExecutor.CallAPI(APIRequestType.Get, this.getDiscountTagsUrl, headerInfo ,
        null, true);
    }
    else
    {
      return this.apiExecutor.CallAPI(APIRequestType.Get, this.getDiscountTagsByFilterconditionUrl, headerInfo ,
        discountTags, true);
    }
  }

  addDiscountTag(discountTags:DiscountTags, headerInfo:HeaderInfo) : Observable<any>
  {
    return this.apiExecutor.CallAPI(APIRequestType.Post, this.addDiscountTagUrl, headerInfo ,
      discountTags, true);
  }

  updateDiscountTag(discountTags:DiscountTags, headerInfo:HeaderInfo) : Observable<any>
  {
    return this.apiExecutor.CallAPI(APIRequestType.Post, this.updateDiscountTagUrl, headerInfo ,
      discountTags, true);
  }

  activateDeactivateDiscountTag(obj:any, headerInfo:HeaderInfo) : Observable<any>
  {
    return this.apiExecutor.CallAPI(APIRequestType.Post, this.activateDeactivateDiscountTagUrl, headerInfo ,
      obj, true);
  }

  deleteDiscountTag(id:number, headerInfo:HeaderInfo) : Observable<any>
  {
    return this.apiExecutor.CallAPI(APIRequestType.Post, this.deleteDiscountTagUrl, headerInfo ,
      id, true);
  }
  //Airlines - DiscountTags mappings
  getAirlineDiscountTagsMapping(airlinesId:number, discountTagId:number, headerInfo:HeaderInfo) : Observable<any>
  {
    if(((airlinesId ?? null) == null || airlinesId === 0 ) 
      && ((discountTagId ?? null) == null || discountTagId === 0 ))
    { 
      return this.apiExecutor.CallAPI(APIRequestType.Get, this.getAirlineDiscountTagsMappingUrl, headerInfo ,
        null, true);
    }
    else
    {
      let requestUrl = this.getAirlinesByFilterconditionUrl.replace(/\\/g, '') + "/" +
      (((airlinesId ?? null) == null || airlinesId === 0 )  ? "0" : airlinesId) + "/" +
      (((discountTagId ?? null) == null || discountTagId === 0 ) ? "0" : discountTagId);
      return this.apiExecutor.CallAPI(APIRequestType.Get, requestUrl, headerInfo ,
        null, true);
    }
  }

  mapAirlinesDiscountTags(map:AirlineDiscountTagMappingDetails[], headerInfo:HeaderInfo) : Observable<any>
  {
    return this.apiExecutor.CallAPI(APIRequestType.Post, this.mapAirlinesDiscountTagsUrl, headerInfo ,
      map, true);
  }

  remapAirlinesDiscountTags(map:RemapAirlineDiscountTagsDetails[], headerInfo:HeaderInfo) : Observable<any>
  {
    return this.apiExecutor.CallAPI(APIRequestType.Post, this.remapAirlinesDiscountTagsUrl, headerInfo ,
      map, true);
  }
  //airline schedules
  getSchedules(id:number|null, headerInfo:HeaderInfo) : Observable<any>
  {
    if(((id ?? null) == null) || id === 0)
    { 
      return this.apiExecutor.CallAPI(APIRequestType.Get, this.getSchedulesUrl, headerInfo ,
        null, true);
    }
    else
    {
      return this.apiExecutor.CallAPI(APIRequestType.Get, 
        this.getSchedulesUrl.replace(/\\/g, '') + "/" + id , headerInfo , null, true);
    }
  }

  getAirlineSchedulesByAirlineId(airlineId:number, headerInfo:HeaderInfo) : Observable<any>
  {
    return this.apiExecutor.CallAPI(APIRequestType.Get, 
      this.getAirlineSchedulesByAirlineIdUrl.replace(/\\/g, '') + "/" + airlineId, headerInfo ,
      null, true);
  }

  getAirlineSchedulesByFilterCondition(airlineSchedule:AirlineSchedules, headerInfo:HeaderInfo) : Observable<any>
  {
    return this.apiExecutor.CallAPI(APIRequestType.Post, this.getAirlineSchedulesByFilterConditionUrl, headerInfo ,
      airlineSchedule, true);
  }

  addSchedule(airlineSchedule:AirlineSchedules, headerInfo:HeaderInfo) : Observable<any>
  {
    return this.apiExecutor.CallAPI(APIRequestType.Post, this.addScheduleUrl, headerInfo ,
      airlineSchedule, true);
  }

  reSchedulesAirlinesByRange(airlineSchedules:AirlineSchedules[], headerInfo:HeaderInfo) : Observable<any>
  {
    return this.apiExecutor.CallAPI(APIRequestType.Post, this.reSchedulesAirlinesByRangeUrl, headerInfo ,
      airlineSchedules, true);
  }

  deleteAirlineSchedule(id:number, headerInfo:HeaderInfo) : Observable<any>
  {
    return this.apiExecutor.CallAPI(APIRequestType.Post, this.deleteAirlineScheduleUrl, headerInfo ,
      id, true);
  }

  deleteAirlineScheduleByIds(ids:number[], headerInfo:HeaderInfo) : Observable<any>
  {
    return this.apiExecutor.CallAPI(APIRequestType.Post, this.deleteAirlineScheduleByIdsUrl, headerInfo ,
      ids, true);
  }

  deleteAirlineScheduleByAirlineIds(airlineIds:number[], headerInfo:HeaderInfo) : Observable<any>
  {
    return this.apiExecutor.CallAPI(APIRequestType.Post, this.deleteAirlineScheduleByAirlineIdsUrl, headerInfo ,
      airlineIds, true);
  }

  // ------------------- getAvailableAirlines -------------------------------------- 
  getAvailableAirlines(airlineScheduleTracker:AirlineScheduleTracker, headerInfo:HeaderInfo) : Observable<any>
  {
    return this.apiExecutor.CallAPI(APIRequestType.Post, this.getAvailableAirlinesUrl, headerInfo ,
      airlineScheduleTracker, true);
  }
}
