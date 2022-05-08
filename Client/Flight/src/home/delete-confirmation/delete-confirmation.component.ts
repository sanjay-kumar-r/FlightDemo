import { Component, OnInit, Output, EventEmitter, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-delete-confirmation',
  templateUrl: './delete-confirmation.component.html',
  styleUrls: ['./delete-confirmation.component.css']
})
export class DeleteConfirmationComponent implements OnInit {

  @Output() isYes : EventEmitter<boolean> = new EventEmitter<boolean>();

  title:string|any;
  itemName:string|any;


  constructor(@Inject(MAT_DIALOG_DATA) public deleteConfirmationData:any,
    private deletePopupRef: MatDialogRef<DeleteConfirmationComponent>) 
  { 
    this.title = deleteConfirmationData?.title ?? "";
    this.itemName = deleteConfirmationData?.itemName ?? "";
  }

  ngOnInit(): void {
    this.title = this.deleteConfirmationData?.title ?? "";
    this.itemName = this.deleteConfirmationData?.itemName ?? "";
  }

  yes()
  {
    this.isYes.emit(true);
    this.closePopup();
  }

  no()
  {
    this.isYes.emit(false);
    this.closePopup();
  }

  closePopup()
  {
    this.deletePopupRef.close();
  }



}
