import { Component, OnInit, signal, TemplateRef, ViewChild, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatMenuModule } from '@angular/material/menu';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';

import { IssueService } from '../../../shared/services/issue.service';
import { ProjectService } from '../../../shared/services/project.service';
import { UserService } from '../../../shared/services/user.service';
import { IssueResponse, ProjectResponse, UserResponse } from '../../../shared/models/models';
import { AuthService } from '../../../core/auth/auth.service';

@Component({
  selector: 'app-issue-list',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    MatSnackBarModule,
    MatMenuModule,
    MatDatepickerModule,
    MatNativeDateModule
  ],
  templateUrl: './issue-list.html',
  styleUrls: ['./issue-list.scss']
})
export class IssueListComponent implements OnInit {
  issues = signal<IssueResponse[]>([]);
  projects = signal<ProjectResponse[]>([]);
  users = signal<UserResponse[]>([]);
  isLoading = signal(false);

  // Filters
  filterForm: FormGroup;
  issueForm: FormGroup;
  editingIssue = signal<IssueResponse | null>(null);

  displayedColumns: string[] = [
    'title', 'project', 'type', 'priority', 'status', 'assignedTo', 'deadline', 'actions'
  ];

  @ViewChild('issueDialog') issueDialogRef!: TemplateRef<any>;
  private dialogRef?: MatDialogRef<any>;

  implementationTypes = ['Angular', 'CSharp', 'SSIS', 'Razor', 'SqlServer', 'Others'];
  issueTypes = ['Requirement', 'Bug', 'Improvement'];
  priorities = ['Low', 'Medium', 'High', 'Critical'];
  statuses = ['Backlog', 'Pending', 'InAnalysis', 'InDevelopment', 'InTesting', 'Done', 'Cancelled'];

  authService = inject(AuthService);
  private issueService = inject(IssueService);
  private projectService = inject(ProjectService);
  private userService = inject(UserService);
  private fb = inject(FormBuilder);
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);
  private router = inject(Router);

  constructor() {
    // Filter form with mandatory ImplementationType defaulting to 'Angular'
    this.filterForm = this.fb.group({
      implementationType: ['Angular', Validators.required],
      projectId: [null],
      status: [null],
      assignedToUserId: [null],
      priority: [null],
      searchTerm: ['']
    });

    // Create/Edit Issue Form
    this.issueForm = this.fb.group({
      projectId: [null, Validators.required],
      title: ['', [Validators.required, Validators.maxLength(250)]],
      description: ['', [Validators.required, Validators.maxLength(4000)]],
      issueType: ['Requirement', Validators.required],
      implementationType: ['Angular', Validators.required],
      requestedBy: ['', [Validators.required, Validators.maxLength(150)]],
      assignedToUserId: [null],
      priority: ['Medium', Validators.required],
      status: ['Backlog', Validators.required],
      startDate: [null],
      endDate: [null],
      deadline: [null]
    });
  }

  ngOnInit(): void {
    this.loadDependencies();
    this.loadIssues();

    // Re-fetch automatically when filters change
    this.filterForm.valueChanges.subscribe(() => {
      this.loadIssues();
    });
  }

  loadDependencies(): void {
    this.projectService.getAll(true).subscribe(res => this.projects.set(res));
    this.userService.getAll().subscribe(res => this.users.set(res));
  }

  loadIssues(): void {
    if (this.filterForm.invalid) return;

    this.isLoading.set(true);
    this.issueService.getFiltered(this.filterForm.value).subscribe({
      next: (res) => {
        this.issues.set(res);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
        this.snackBar.open('Erro ao carregar demandas.', 'Fechar', { duration: 3000 });
      }
    });
  }

  navigateToDetail(id: number): void {
    this.router.navigate(['/issues', id]);
  }

  openCreateDialog(): void {
    this.editingIssue.set(null);
    this.issueForm.reset({
      issueType: 'Requirement',
      implementationType: this.filterForm.value.implementationType || 'Angular',
      priority: 'Medium',
      status: 'Backlog',
      requestedBy: this.authService.currentUser() || ''
    });
    this.dialogRef = this.dialog.open(this.issueDialogRef, { width: '700px' });
  }

  openEditDialog(event: Event, issue: IssueResponse): void {
    event.stopPropagation(); // prevent navigating to detail
    this.editingIssue.set(issue);
    this.issueForm.patchValue({
      projectId: issue.projectId,
      title: issue.title,
      description: issue.description,
      issueType: issue.issueType,
      implementationType: issue.implementationType,
      requestedBy: issue.requestedBy,
      assignedToUserId: issue.assignedToUserId,
      priority: issue.priority,
      status: issue.status,
      startDate: issue.startDate,
      endDate: issue.endDate,
      deadline: issue.deadline
    });
    this.dialogRef = this.dialog.open(this.issueDialogRef, { width: '700px' });
  }

  saveIssue(): void {
    if (this.issueForm.invalid) {
      this.issueForm.markAllAsTouched();
      const missingProject = !this.issueForm.get('projectId')?.value;
      if (missingProject) {
        this.snackBar.open('Selecione um Projeto antes de salvar.', 'Fechar', { duration: 4000 });
      }
      return;
    }

    const request = this.issueForm.value;
    const editing = this.editingIssue();

    if (editing) {
      this.issueService.update(editing.id, request).subscribe({
        next: () => {
          this.snackBar.open('Demanda atualizada com sucesso!', 'Fechar', { duration: 3000 });
          this.dialogRef?.close();
          this.loadIssues();
        },
        error: (err: HttpErrorResponse) => {
          const msg = err.error?.message ?? err.error?.title ?? 'Erro ao atualizar demanda.';
          this.snackBar.open(msg, 'Fechar', { duration: 5000 });
        }
      });
    } else {
      this.issueService.create(request).subscribe({
        next: () => {
          this.snackBar.open('Demanda criada com sucesso!', 'Fechar', { duration: 3000 });
          this.dialogRef?.close();
          this.loadIssues();
        },
        error: (err: HttpErrorResponse) => {
          const msg = err.error?.message ?? err.error?.title ?? 'Erro ao criar demanda.';
          this.snackBar.open(msg, 'Fechar', { duration: 5000 });
        }
      });
    }
  }

  deleteIssue(event: Event, id: number): void {
    event.stopPropagation();
    if (!confirm('Deseja realmente remover esta demanda?')) return;

    this.issueService.delete(id).subscribe({
      next: () => {
        this.snackBar.open('Demanda removida com sucesso!', 'Fechar', { duration: 3000 });
        this.loadIssues();
      },
      error: () => this.snackBar.open('Erro ao remover demanda.', 'Fechar', { duration: 3000 })
    });
  }

  changePriority(event: Event, issue: IssueResponse, priority: string): void {
    event.stopPropagation();
    this.issueService.updatePriority(issue.id, priority).subscribe({
      next: () => {
        this.snackBar.open('Prioridade atualizada!', 'Fechar', { duration: 2000 });
        this.loadIssues();
      },
      error: () => this.snackBar.open('Erro ao atualizar prioridade.', 'Fechar', { duration: 3000 })
    });
  }

  changeStatus(event: Event, issue: IssueResponse, status: string): void {
    event.stopPropagation();
    this.issueService.updateStatus(issue.id, status, issue.assignedToUserId).subscribe({
      next: () => {
        this.snackBar.open('Status atualizado!', 'Fechar', { duration: 2000 });
        this.loadIssues();
      },
      error: () => this.snackBar.open('Erro ao atualizar status.', 'Fechar', { duration: 3000 })
    });
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
    
    return diffDays >= 0 && diffDays <= 3;
  }
}
