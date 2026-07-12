import { Component, Inject, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';

@Component({
  selector: 'app-timesheet-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatDatepickerModule,
    MatNativeDateModule
  ],
  templateUrl: './timesheet-dialog.html',
  styleUrls: ['./timesheet-dialog.scss']
})
export class TimesheetDialogComponent {
  timesheetForm: FormGroup;
  
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<TimesheetDialogComponent>);

  constructor(@Inject(MAT_DIALOG_DATA) public data: { issueId: number }) {
    this.timesheetForm = this.fb.group({
      issueId: [data.issueId, Validators.required],
      loggedDate: [new Date(), Validators.required],
      hoursSpent: [1, [Validators.required, Validators.min(0.1), Validators.max(24)]],
      workDescription: ['', [Validators.required, Validators.maxLength(1000)]]
    });
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (this.timesheetForm.invalid) return;
    this.dialogRef.close(this.timesheetForm.value);
  }
}
