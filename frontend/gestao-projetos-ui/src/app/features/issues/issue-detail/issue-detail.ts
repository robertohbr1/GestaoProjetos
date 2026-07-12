import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatTabsModule } from '@angular/material/tabs';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';

import { IssueService } from '../../../shared/services/issue.service';
import { ProjectService } from '../../../shared/services/project.service';
import { UserService } from '../../../shared/services/user.service';
import { TimesheetService } from '../../../shared/services/timesheet.service';
import { CommentService } from '../../../shared/services/comment.service';
import { AttachmentService } from '../../../shared/services/attachment.service';
import { IssueDetailResponse, ProjectResponse, UserResponse } from '../../../shared/models/models';
import { AuthService } from '../../../core/auth/auth.service';
import { TimesheetDialogComponent } from '../timesheet-dialog/timesheet-dialog';

@Component({
  selector: 'app-issue-detail',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatTabsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule,
    MatDialogModule,
    MatDatepickerModule,
    MatNativeDateModule
  ],
  templateUrl: './issue-detail.html',
  styleUrls: ['./issue-detail.scss']
})
export class IssueDetailComponent implements OnInit {
  issue = signal<IssueDetailResponse | null>(null);
  projects = signal<ProjectResponse[]>([]);
  users = signal<UserResponse[]>([]);
  isLoading = signal(false);

  infoForm!: FormGroup;
  commentForm: FormGroup;

  implementationTypes = ['Angular', 'CSharp', 'SSIS', 'Razor', 'SqlServer', 'Others'];
  issueTypes = ['Requirement', 'Bug', 'Improvement'];
  priorities = ['Low', 'Medium', 'High', 'Critical'];
  statuses = ['Backlog', 'Pending', 'InAnalysis', 'InDevelopment', 'InTesting', 'Done', 'Cancelled'];

  authService = inject(AuthService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private issueService = inject(IssueService);
  private projectService = inject(ProjectService);
  private userService = inject(UserService);
  private timesheetService = inject(TimesheetService);
  private commentService = inject(CommentService);
  private attachmentService = inject(AttachmentService);
  private fb = inject(FormBuilder);
  private snackBar = inject(MatSnackBar);
  private dialog = inject(MatDialog);

  constructor() {
    this.commentForm = this.fb.group({
      content: ['', [Validators.required, Validators.maxLength(2000)]]
    });
  }

  ngOnInit(): void {
    this.loadDependencies();
    this.route.params.subscribe(params => {
      const id = +params['id'];
      if (id) {
        this.loadIssue(id);
      }
    });
  }

  loadDependencies(): void {
    this.projectService.getAll(true).subscribe(res => this.projects.set(res));
    this.userService.getAll().subscribe(res => this.users.set(res));
  }

  loadIssue(id: number): void {
    this.isLoading.set(true);
    this.issueService.getById(id).subscribe({
      next: (res) => {
        this.issue.set(res);
        this.initInfoForm(res);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
        this.snackBar.open('Erro ao carregar detalhes da demanda.', 'Fechar', { duration: 3000 });
        this.router.navigate(['/issues']);
      }
    });
  }

  initInfoForm(issue: IssueDetailResponse): void {
    this.infoForm = this.fb.group({
      projectId: [issue.projectId, Validators.required],
      title: [issue.title, [Validators.required, Validators.maxLength(250)]],
      description: [issue.description, [Validators.required, Validators.maxLength(4000)]],
      issueType: [issue.issueType, Validators.required],
      implementationType: [issue.implementationType, Validators.required],
      requestedBy: [issue.requestedBy, [Validators.required, Validators.maxLength(150)]],
      assignedToUserId: [issue.assignedToUserId],
      priority: [issue.priority, Validators.required],
      status: [issue.status, Validators.required],
      startDate: [issue.startDate],
      endDate: [issue.endDate],
      deadline: [issue.deadline]
    });
  }

  saveInfo(): void {
    const issueData = this.issue();
    if (!issueData || this.infoForm.invalid) return;

    this.issueService.update(issueData.id, this.infoForm.value).subscribe({
      next: () => {
        this.snackBar.open('Informações atualizadas com sucesso!', 'Fechar', { duration: 3000 });
        this.loadIssue(issueData.id);
      },
      error: () => this.snackBar.open('Erro ao atualizar demanda.', 'Fechar', { duration: 3000 })
    });
  }

  // TIMESHEET
  openTimesheetDialog(): void {
    const issueData = this.issue();
    if (!issueData) return;

    const dialogRef = this.dialog.open(TimesheetDialogComponent, {
      width: '450px',
      data: { issueId: issueData.id }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.timesheetService.create(result).subscribe({
          next: () => {
            this.snackBar.open('Horas apontadas com sucesso!', 'Fechar', { duration: 3000 });
            this.loadIssue(issueData.id);
          },
          error: () => this.snackBar.open('Erro ao apontar horas.', 'Fechar', { duration: 3000 })
        });
      }
    });
  }

  deleteTimeLog(id: number): void {
    const issueData = this.issue();
    if (!issueData || !confirm('Deseja remover este apontamento?')) return;

    this.timesheetService.delete(id).subscribe({
      next: () => {
        this.snackBar.open('Lançamento removido!', 'Fechar', { duration: 3000 });
        this.loadIssue(issueData.id);
      },
      error: () => this.snackBar.open('Erro ao remover lançamento.', 'Fechar', { duration: 3000 })
    });
  }

  // COMMENTS
  addComment(): void {
    const issueData = this.issue();
    if (!issueData || this.commentForm.invalid) return;

    this.commentService.create(issueData.id, this.commentForm.value.content).subscribe({
      next: () => {
        this.snackBar.open('Comentário postado!', 'Fechar', { duration: 3000 });
        this.commentForm.reset();
        this.loadIssue(issueData.id);
      },
      error: () => this.snackBar.open('Erro ao postar comentário.', 'Fechar', { duration: 3000 })
    });
  }

  deleteComment(id: number): void {
    const issueData = this.issue();
    if (!issueData || !confirm('Deseja remover este comentário?')) return;

    this.commentService.delete(id).subscribe({
      next: () => {
        this.snackBar.open('Comentário removido!', 'Fechar', { duration: 3000 });
        this.loadIssue(issueData.id);
      },
      error: () => this.snackBar.open('Erro ao remover comentário.', 'Fechar', { duration: 3000 })
    });
  }

  // ATTACHMENTS
  onFileSelected(event: any): void {
    const file: File = event.target.files[0];
    const issueData = this.issue();
    if (!file || !issueData) return;

    this.attachmentService.upload(issueData.id, file).subscribe({
      next: () => {
        this.snackBar.open('Arquivo anexado com sucesso!', 'Fechar', { duration: 3000 });
        this.loadIssue(issueData.id);
      },
      error: () => this.snackBar.open('Erro ao enviar arquivo.', 'Fechar', { duration: 3000 })
    });
  }

  downloadAttachment(id: number, name: string): void {
    this.attachmentService.download(id).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = name;
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: () => this.snackBar.open('Erro ao baixar arquivo.', 'Fechar', { duration: 3000 })
    });
  }

  deleteAttachment(id: number): void {
    const issueData = this.issue();
    if (!issueData || !confirm('Deseja remover este anexo?')) return;

    this.attachmentService.delete(id).subscribe({
      next: () => {
        this.snackBar.open('Anexo removido!', 'Fechar', { duration: 3000 });
        this.loadIssue(issueData.id);
      },
      error: () => this.snackBar.open('Erro ao remover anexo.', 'Fechar', { duration: 3000 })
    });
  }

  deleteIssue(): void {
    const issueData = this.issue();
    if (!issueData || !confirm('Confirmar a remoção permanente desta demanda?')) return;

    this.issueService.delete(issueData.id).subscribe({
      next: () => {
        this.snackBar.open('Demanda removida!', 'Fechar', { duration: 3000 });
        this.router.navigate(['/issues']);
      },
      error: () => this.snackBar.open('Erro ao remover demanda.', 'Fechar', { duration: 3000 })
    });
  }
}
