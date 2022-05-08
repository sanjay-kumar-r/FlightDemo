import { Component, OnInit, Output, EventEmitter, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { AirlineDiscountTagMappingDetails, Airlines, DiscountTags, RemapAirlineDiscountTagsDetails } from 'src/Models/Airlines';
import { HeaderInfo } from 'src/Models/HeaderInfo';
import { AuthResponse, Users } from 'src/Models/Users';
import { AirlineService } from 'src/Services/airline.service';
import { CommonService } from 'src/Services/common.service';

@Component({
  selector: 'app-airline-discount-tag-mapping',
  templateUrl: './airline-discount-tag-mapping.component.html',
  styleUrls: ['./airline-discount-tag-mapping.component.css']
})
export class AirlineDiscountTagMappingComponent implements OnInit {

  @Output() airlineSaveResult : EventEmitter<boolean> = new EventEmitter<boolean>(); 
  @Output() isErrorOutput : EventEmitter<boolean> = new EventEmitter<boolean>(); 
  @Output() isCancel : EventEmitter<boolean> = new EventEmitter<boolean>(); 
  @Output() isSaveTrigger : EventEmitter<boolean> = new EventEmitter<boolean>(); 

  airlineName:string = "";
  airlineId?:number|null;
  //mappingDetails?:AirlineDiscountTagMappingDetails[]|null;
  assignedDiscountTags?:DiscountTags[]|null;
  existingDiscountTags?:DiscountTags[]|null;
  addedDiscountTagIds:number[];
  removedDiscountTagIds:number[];

  selectedListFromAssigned:number[];
  selectedListFromExisting:number[];


  isFormSubmitted:boolean = false;
  isDuplicate:boolean = false;
  isSeatsInvalid:boolean = false;
  isCanceled:boolean = false;
  isSavedTriggered:boolean = false;

  userDetails:Users|null;
  authResponse:AuthResponse|null;
  headerInfo?:HeaderInfo|any;

  constructor(@Inject(MAT_DIALOG_DATA) public mappingInputData:any,
    private airlineDiscountTagMappingCompRef: MatDialogRef<AirlineDiscountTagMappingComponent>,
    private airlineService:AirlineService,
    private commonService:CommonService,
    private router:Router
    ) 
  {
    this.airlineId = 0;
    this.airlineName = "";
    this.assignedDiscountTags = [];
    this.existingDiscountTags = [];
    this.addedDiscountTagIds = [];
    this.removedDiscountTagIds = [];

    this.selectedListFromAssigned = [];
    this.selectedListFromExisting = [];

    this.userDetails = null;
    this.authResponse = null;
    this.headerInfo = null;
  }

  ngOnInit(): void {
    this.getUserDetails(JSON.parse(localStorage.getItem("userDetails") ?? "{}"));
    this.getAuthResponse(JSON.parse(localStorage.getItem("authResponse") ?? "{}"));
    this.getHeader();
    this.addedDiscountTagIds = [];
    this.removedDiscountTagIds = [];

    this.selectedListFromAssigned = [];
    this.selectedListFromExisting = [];

    if((this.mappingInputData ?? null) != null)
    {
      this.airlineId = this.mappingInputData?.airlineId ?? 0;
      if((this.airlineId ?? 0) >= 0)
      {
        this.airlineName = this.mappingInputData?.airlineName ?? "";
        //get mapping
        this.airlineService.getAirlineDiscountTagsMapping(this.airlineId ?? -1, -1, this.headerInfo)
        .subscribe(
          (result) => {
            if(((result ?? null) != null) && ((result?.length ?? 0) > 0))
              this.getAssignedDiscountTags(result[0]);
            //get existing discountTags
            this.loadExistingDiscountTag();
          },
          (error) =>{
            if(error.status == 401)
            {
              //refreshToken
              this.commonService.refreshToken(this.headerInfo).subscribe(
                (authResponse) =>{
                  let refreshResult:AuthResponse = {
                    IsSuccess: authResponse.isSuccess,
                    Token : authResponse.token,
                    RefreshToken : authResponse.refreshToken,
                    Reason: authResponse.reason
                  };
                  localStorage.setItem("authResponse", JSON.stringify(refreshResult));
                  this.getAuthResponse(JSON.parse(localStorage.getItem("authResponse") ?? "{}"));
                  this.getHeader();
                  //this.headerInfo.Authorization = `Bearer ${refreshResult.Token}`;
                  //retry
                  this.airlineService.getAirlineDiscountTagsMapping(this.airlineId ?? -1, -1, this.headerInfo).subscribe(
                    (result) =>{
                      if(((result ?? null) != null) && ((result?.length ?? 0) > 0))
                        this.getAssignedDiscountTags(result[0]);
                      //get existing discountTags
                      this.loadExistingDiscountTag();
                    },
                    (e) =>{
                      console.error(e);
                      this.emitError();  
                    }
                  );
                },
                (refreshError) => {
                  console.error(refreshError);
                  this.emitError();
                }
              );
            }
            else
            {
              console.error(error);
              this.emitError();
            }
          }
        );
      }
      else
      {
        console.error("airline id is not present");
        this.emitError();
      }
    }
    else
    {
      this.emitError();
    }
  }

  loadExistingDiscountTag(){
    this.airlineService.getDiscountTags(null, this.headerInfo).subscribe(
      (result) =>{
        this.getExistingDiscountTagList(result);
      },
      (error) => {
        if(error.status == 401)
        {
          //refreshToken
          this.commonService.refreshToken(this.headerInfo).subscribe(
            (authResponse) =>{
              let refreshResult:AuthResponse = {
                IsSuccess: authResponse.isSuccess,
                Token : authResponse.token,
                RefreshToken : authResponse.refreshToken,
                Reason: authResponse.reason
              };
              localStorage.setItem("authResponse", JSON.stringify(refreshResult));
              this.getAuthResponse(JSON.parse(localStorage.getItem("authResponse") ?? "{}"));
              this.getHeader();
              //this.headerInfo.Authorization = `Bearer ${refreshResult.Token}`;
              //retry
              this.airlineService.getDiscountTags(null, this.headerInfo).subscribe(
                (result) =>{
                  this.getExistingDiscountTagList(result);
                },
                (e) =>{
                  console.error(e);
                  this.emitError();  
                }
              );
            },
            (refreshError) => {
              console.error(refreshError);
              this.emitError();
            }
          );
        }
        else
        {
          console.error(error);
          this.emitError();
        }
      }
    );
  }

  onSelectItem($event:any, from:string)
  {
    if(from.toLowerCase() === "assigned")
    {
      if(((this.selectedListFromAssigned ?? null) == null))
        this.selectedListFromAssigned = [];
      if($event.target.classList.contains("selectedDiscout"))
      {
        //remove
        let _id:string = $event.target.getAttribute("discountId".toLowerCase()) ?? "0";
        let id:number = Number(_id);
        this.selectedListFromAssigned = this.selectedListFromAssigned.filter(x => x !== id);
        //uncheck
        $event.target.classList.remove("selectedDiscout");
      }
      else
      {
        //add
        let _id:string = $event.target.getAttribute("discountId".toLowerCase()) ?? "0";
        let id:number = Number(_id);
        if(!this.selectedListFromAssigned.some(x => x === id))
          this.selectedListFromAssigned.push(id);
        //check
        $event.target.classList.add("selectedDiscout");
      }
    }
    else if(from.toLowerCase() === "existing")
    {
      if(((this.selectedListFromExisting ?? null) == null))
        this.selectedListFromExisting = [];
      if($event.target.classList.contains("selectedDiscout"))
      {
        //remove
        let _id:string = $event.target.getAttribute("discountId".toLowerCase()) ?? "0";
        let id:number = Number(_id);
        this.selectedListFromExisting = this.selectedListFromExisting.filter(x => x !== id);
        //uncheck
        $event.target.classList.remove("selectedDiscout");
      }
      else
      {
        //add
        let _id:string = $event.target.getAttribute("discountId".toLowerCase()) ?? "0";
        let id:number = Number(_id);
        if(!this.selectedListFromExisting.some(x => x === id))
          this.selectedListFromExisting.push(id);
        //check
        $event.target.classList.add("selectedDiscout");
      }
    }
  }

  removeItems(){
    if(((this.selectedListFromAssigned ?? null) == null) 
      && ((this.selectedListFromAssigned?.length ?? 0) <= 0))
    {    
      this.selectedListFromAssigned = [];
      return;
    }
    if(((this.removedDiscountTagIds ?? null) == null))
      this.removedDiscountTagIds = [];
    if(((this.existingDiscountTags ?? null) == null))
      this.existingDiscountTags = [];
    if(((this.assignedDiscountTags ?? null) == null))
      this.assignedDiscountTags = [];

    //add to removed list
    //add to  existing DiscountDetails list
    this.selectedListFromAssigned.forEach(x => {
      if(!this.removedDiscountTagIds.some(y => y === x))
        this.removedDiscountTagIds.push(x);
      let obj:DiscountTags|any = this.assignedDiscountTags?.find(y => (y.Id ?? 0) === x);
      if(((obj ?? null) != null))
      {
        if(!this.existingDiscountTags?.some(y => (y.Id ?? 0) === x))
          this.existingDiscountTags?.push(obj);
      }
    });

    //remove from added list
    if(((this.addedDiscountTagIds ?? null) == null))
      this.addedDiscountTagIds = [];
    else if(((this.addedDiscountTagIds ?? null) != null) 
      && ((this.addedDiscountTagIds?.length ?? 0) > 0))
    {
      this.addedDiscountTagIds = this.addedDiscountTagIds.filter(x => 
        !this.selectedListFromAssigned.some(y => y === x)
        );
    }

    //remove from assigned DiscountTagDetails list
    if(((this.assignedDiscountTags ?? null) != null) 
      && ((this.assignedDiscountTags?.length ?? 0) > 0))
    {
      this.assignedDiscountTags = this.assignedDiscountTags?.filter(x => 
        !this.selectedListFromAssigned.some(y => y === (x.Id ?? 0))
        );
    }

    //clear selected from assigned list
    this.selectedListFromAssigned = [];
  }

  addItems()
  {
    if(((this.selectedListFromExisting ?? null) == null) 
      && ((this.selectedListFromExisting?.length ?? 0) <= 0))
    {    
      this.selectedListFromExisting = [];
      return;
    }
    if(((this.addedDiscountTagIds ?? null) == null))
      this.addedDiscountTagIds = [];
    if(((this.existingDiscountTags ?? null) == null))
      this.existingDiscountTags = [];
    if(((this.assignedDiscountTags ?? null) == null))
      this.assignedDiscountTags = [];

    //add to added list
    //add to  assigned DiscountDetails list
    this.selectedListFromExisting.forEach(x => {
      if(!this.addedDiscountTagIds.some(y => y === x))
        this.addedDiscountTagIds.push(x);
      let obj:DiscountTags|any = this.existingDiscountTags?.find(y => (y.Id ?? 0) === x);
      if(((obj ?? null) != null))
      {
        if(!this.assignedDiscountTags?.some(y => (y.Id ?? 0) === x))
          this.assignedDiscountTags?.push(obj);
      }
    });
    
    //remove from removed list
    if(((this.removedDiscountTagIds ?? null) == null))
      this.removedDiscountTagIds = [];
    else if(((this.removedDiscountTagIds ?? null) != null) 
      && ((this.removedDiscountTagIds?.length ?? 0) > 0))
    {
      this.removedDiscountTagIds = this.removedDiscountTagIds.filter(x => 
        !this.selectedListFromExisting.some(y => y === x)
        );
    }

    //remove from exixting DiscountTagDetails list
    if(((this.existingDiscountTags ?? null) != null) 
      && ((this.existingDiscountTags?.length ?? 0) > 0))
    {
      this.existingDiscountTags = this.existingDiscountTags?.filter(x => 
        !this.selectedListFromExisting.some(y => y === (x.Id ?? 0))
        );
    }

    //clear selected from existing list
    this.selectedListFromExisting = [];
  }

  saveChanges()
  {
    //save already triggered
    if(this.isSavedTriggered)
      return;
    
    if(
      (((this.addedDiscountTagIds ?? null) == null) 
      || ((this.addedDiscountTagIds?.length ?? 0) <= 0))
      &&(((this.removedDiscountTagIds ?? null) == null) 
      || ((this.removedDiscountTagIds?.length ?? 0) <= 0))
      )
    {
      alert("no change made");
      return;
    }

    this.isSavedTriggered = true;
    let remapDetails:RemapAirlineDiscountTagsDetails[] = [
      {
        AirlineId : this.airlineId ?? 0,
        AddedDiscountTagIds: this.addedDiscountTagIds,
        RemovedDiscountTagIds: this.removedDiscountTagIds
      }
    ];
    this.airlineService.remapAirlinesDiscountTags(remapDetails, this.headerInfo).subscribe(
      (result) => {
        if(((result ?? null) != null) && ((result ?? false) == true))
        {
          alert("airline discount Tag mapping added sucessfully");
          this.saveAndClose();
        }
        else
        {
          this.emitError();
        }     
      },
      (error) =>{
        if(error.status == 401)
        {
          //refreshToken
          this.commonService.refreshToken(this.headerInfo).subscribe(
            (authResponse) =>{
              let refreshResult:AuthResponse = {
                IsSuccess: authResponse.isSuccess,
                Token : authResponse.token,
                RefreshToken : authResponse.refreshToken,
                Reason: authResponse.reason
              };
              localStorage.setItem("authResponse", JSON.stringify(refreshResult));
              this.getAuthResponse(JSON.parse(localStorage.getItem("authResponse") ?? "{}"));
              this.getHeader();
              //this.headerInfo.Authorization = `Bearer ${refreshResult.Token}`;
              //retry
              this.airlineService.remapAirlinesDiscountTags(remapDetails, this.headerInfo).subscribe(
                (result) =>{
                  if(((result ?? null) != null) && ((result ?? false) == true))
                  {
                    alert("airline discount Tag mapping added sucessfully");
                    this.saveAndClose();
                  }
                  else
                  {
                    this.emitError();
                  }
                },
                (e) =>{
                  console.error(e);
                  this.emitError();  
                }
              );
            },
            (refreshError) => {
              console.error(refreshError);
              this.emitError();
            }
          );
        }
        else
        {
          console.error(error);
          this.emitError();
        }
      }
    );

   
  }

  getHeader()
  {
    this.headerInfo = {
      UserId: this.userDetails?.Id?.toString() ?? "",
      Authorization: `Bearer ${this.authResponse?.Token}`,
      RefreshToken: this.authResponse?.RefreshToken ?? ""
    };
  }

  getAuthResponse(authResponse:any)
  {
    if(((authResponse ?? null) != null) && ((authResponse?.Token ?? null) != null)
      && ((authResponse?.RefreshToken ?? null) != null))
    {
      this.authResponse = authResponse;
    }
  }

  getUserDetails(user:any)
  {
    if(((user ?? null) != null) && ((user?.Id ?? null) != null) && (user.Id != 0))
    {
      this.userDetails = user;
    }
  }

  getAssignedDiscountTags(mappingResponse:any|null){
    if(((mappingResponse ?? null) != null))
    {
      this.assignedDiscountTags = [];
      let discountTags:any[] = mappingResponse.discountTags;
      if(((discountTags ?? null) != null) && ((discountTags?.length ?? 0) > 0))
      {
        discountTags.forEach(x => 
        {
          if(((discountTags ?? null) != null))
            this.assignedDiscountTags?.push(this.getDiscountTag(x));
        });
      }
    }
  }

  // getMappingDetail(mapping:any)
  // {
  //   let airline = mapping.airline;
  //   let discountTagDetails:DiscountTags[]=[]; 
  //   let discountTags:any[] = mapping.discountTagDetails;
  //   discountTags.forEach(discountTag => {
  //     let res:DiscountTags = {
  //       Id : discountTag.id,
  //       Name : discountTag.name,
  //       DiscountCode: discountTag.discountCode,
  //       Description: discountTag.description,
  //       Discount: discountTag.discount ?? 0,
  //       IsByRate: discountTag?.isByRate ?? false,
  //       IsActive: discountTag?.isActive ?? false,
  //       CreatedOn: discountTag.createdOn,
  //       ModifiedOn: discountTag.modifiedOn
  //     }
  //     discountTagDetails.push(res)
  //   });
  //   let res:AirlineDiscountTagMappingDetails = {
  //     Airline : {
  //       Id : airline.id,
  //       Name : airline.name,
  //       AirlineCode: airline.airlineCode,
  //       ContactNumber: airline.contactNumber,
  //       ContactAddress: airline.contactAddress,
  //       TotalSeats: airline.totalSeats,
  //       TotalBCSeats: airline.totalBCSeats,
  //       TotalNBCSeats: airline.totalNBCSeats,
  //       BCTicketCost: airline.bcTicketCost,
  //       NBCTicketCost: airline.nbcTicketCost,
  //       IsActive: airline.isActive ?? false,
  //       CreatedOn: airline.createdOn,
  //       ModifiedOn: airline.modifiedOn
  //     },
  //     DiscountTagDetails: discountTagDetails
  //   };
  //   return res;
  // }

  getExistingDiscountTagList(discountTags:any[]|null)
  {
    if(((discountTags ?? null) != null) && ((discountTags?.length ?? 0) > 0) )
    {
      this.existingDiscountTags = [];
      discountTags?.forEach(x => 
      {
        if(((x ?? null) != null))
        {
          let discountTag = this.getDiscountTag(x);
          if(((this.assignedDiscountTags ?? null) != null) && ((this.assignedDiscountTags?.length ?? 0) > 0)
            && !(this.assignedDiscountTags?.some(x => x.Id === discountTag.Id)))
          {
            this.existingDiscountTags?.push(this.getDiscountTag(x));
          }
        }
      });
    }
  }

  getDiscountTag(discountTag:any)
  {
    let res:DiscountTags = {
      Id : discountTag.id,
      Name : discountTag.name,
      DiscountCode: discountTag.discountCode,
      Description: discountTag.description,
      Discount: discountTag.discount ?? 0,
      IsByRate: discountTag?.isByRate ?? false,
      IsActive: discountTag?.isActive ?? false,
      CreatedOn: discountTag.createdOn,
      ModifiedOn: discountTag.modifiedOn
    }
    return res;
  }

  emitError()
  {
    this.isErrorOutput.emit(true);
    this.closePopup();
  }

  closePopup(){
    this.airlineDiscountTagMappingCompRef.close();
  }

  saveAndClose(){
    this.isSavedTriggered = true;
    this.isSaveTrigger.emit(true);
    this.closePopup();
  }

  cancel()
  {
    this.isCanceled = true;
    this.isCancel.emit(true);
    this.closePopup();
  }

}
