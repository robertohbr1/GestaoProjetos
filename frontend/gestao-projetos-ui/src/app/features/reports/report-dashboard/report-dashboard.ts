import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

import { ReportService } from '../../../shared/services/report.service';
import { DashboardSummary, DeveloperWorkload, IssueResponse } from '../../../shared/models/models';

@Component({
  selector: 'app-report-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSnackBarModule
  ],
  templateUrl: './report-dashboard.html',
  styleUrls: ['./report-dashboard.scss']
})
export class ReportDashboardComponent implements OnInit {
  summary = signal<DashboardSummary | null>(null);
  completedIssues = signal<IssueResponse[]>([]);
  workload = signal<DeveloperWorkload[]>([]);
  pendingIssues = signal<IssueResponse[]>([]);
  isLoading = signal(false);

  dateRangeForm: FormGroup;
  today = new Date();

  private reportService = inject(ReportService);
  private fb = inject(FormBuilder);
  private snackBar = inject(MatSnackBar);

  constructor() {
    // Default period: last 30 days
    const startDate = new Date();
    startDate.setDate(startDate.getDate() - 30);
    const endDate = new Date();

    this.dateRangeForm = this.fb.group({
      startDate: [startDate],
      endDate: [endDate]
    });
  }

  ngOnInit(): void {
    this.loadSummary();
    this.loadCompletedReport();
    this.loadWorkload();
    this.loadPendingReport();
  }

  loadSummary(): void {
    this.reportService.getSummary().subscribe({
      next: (res) => this.summary.set(res),
      error: () => this.snackBar.open('Erro ao carregar sumário.', 'Fechar', { duration: 3000 })
    });
  }

  loadCompletedReport(): void {
    const { startDate, endDate } = this.dateRangeForm.value;
    if (!startDate || !endDate) return;

    // Convert to ISO string without time
    const startStr = startDate.toISOString().split('T')[0];
    const endStr = endDate.toISOString().split('T')[0];

    this.reportService.getCompleted(startStr, endStr).subscribe({
      next: (res) => this.completedIssues.set(res),
      error: () => this.snackBar.open('Erro ao carregar relatório de concluídos.', 'Fechar', { duration: 3000 })
    });
  }

  loadWorkload(): void {
    this.reportService.getWorkload().subscribe({
      next: (res) => this.workload.set(res),
      error: () => this.snackBar.open('Erro ao carregar carga de trabalho.', 'Fechar', { duration: 3000 })
    });
  }

  loadPendingReport(): void {
    this.reportService.getPending().subscribe({
      next: (res) => this.pendingIssues.set(res),
      error: () => this.snackBar.open('Erro ao carregar pendências.', 'Fechar', { duration: 3000 })
    });
  }

  onPeriodSubmit(): void {
    this.loadCompletedReport();
  }

  isOverdue(deadlineStr?: string): boolean {
    if (!deadlineStr) return false;
    const deadline = new Date(deadlineStr);
    deadline.setHours(0, 0, 0, 0);
    const curDate = new Date();
    curDate.setHours(0, 0, 0, 0);
    return deadline < curDate;
  }

  isCloseDeadline(deadlineStr?: string): boolean {
    if (!deadlineStr) return false;
    const deadline = new Date(deadlineStr);
    deadline.setHours(0, 0, 0, 0);
    const curDate = new Date();
    curDate.setHours(0, 0, 0, 0);
    
    const diffTime = deadline.getTime() - curDate.getTime();
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    
    return diffDays >= 0 && diffDays <= 3; // today or next 3 days
  }
}
