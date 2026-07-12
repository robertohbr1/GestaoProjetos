import { Component, OnInit, signal, TemplateRef, ViewChild, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ProjectService } from '../../../shared/services/project.service';
import { ProjectResponse } from '../../../shared/models/models';
import { AuthService } from '../../../core/auth/auth.service';

@Component({
  selector: 'app-project-list',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatCheckboxModule,
    MatSnackBarModule
  ],
  templateUrl: './project-list.html',
  styleUrls: ['./project-list.scss']
})
export class ProjectListComponent implements OnInit {
  projects = signal<ProjectResponse[]>([]);
  isLoading = signal(false);
  
  displayedColumns: string[] = ['name', 'description', 'status', 'createdAt', 'actions'];
  projectForm: FormGroup;
  editingProject = signal<ProjectResponse | null>(null);

  @ViewChild('projectDialog') projectDialogRef!: TemplateRef<any>;
  private dialogRef?: MatDialogRef<any>;

  authService = inject(AuthService);
  private projectService = inject(ProjectService);
  private fb = inject(FormBuilder);
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);

  constructor() {
    this.projectForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(150)]],
      description: ['', [Validators.required, Validators.maxLength(1000)]],
      isActive: [true]
    });
  }

  ngOnInit(): void {
    this.loadProjects();
  }

  loadProjects(): void {
    this.isLoading.set(true);
    this.projectService.getAll().subscribe({
      next: (res) => {
        this.projects.set(res);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
        this.snackBar.open('Erro ao carregar projetos.', 'Fechar', { duration: 3000 });
      }
    });
  }

  openCreateDialog(): void {
    this.editingProject.set(null);
    this.projectForm.reset({ isActive: true });
    this.dialogRef = this.dialog.open(this.projectDialogRef, { width: '500px' });
  }

  openEditDialog(project: ProjectResponse): void {
    this.editingProject.set(project);
    this.projectForm.patchValue({
      name: project.name,
      description: project.description,
      isActive: project.isActive
    });
    this.dialogRef = this.dialog.open(this.projectDialogRef, { width: '500px' });
  }

  saveProject(): void {
    if (this.projectForm.invalid) return;

    const request = this.projectForm.value;
    const editing = this.editingProject();

    if (editing) {
      this.projectService.update(editing.id, request).subscribe({
        next: () => {
          this.snackBar.open('Projeto atualizado com sucesso!', 'Fechar', { duration: 3000 });
          this.dialogRef?.close();
          this.loadProjects();
        },
        error: () => this.snackBar.open('Erro ao atualizar projeto.', 'Fechar', { duration: 3000 })
      });
    } else {
      this.projectService.create(request).subscribe({
        next: () => {
          this.snackBar.open('Projeto criado com sucesso!', 'Fechar', { duration: 3000 });
          this.dialogRef?.close();
          this.loadProjects();
        },
        error: () => this.snackBar.open('Erro ao criar projeto.', 'Fechar', { duration: 3000 })
      });
    }
  }

  deleteProject(id: number): void {
    if (!confirm('Deseja realmente remover este projeto? As demandas associadas serão removidas.')) return;

    this.projectService.delete(id).subscribe({
      next: () => {
        this.snackBar.open('Projeto removido com sucesso!', 'Fechar', { duration: 3000 });
        this.loadProjects();
      },
      error: () => this.snackBar.open('Erro ao remover projeto.', 'Fechar', { duration: 3000 })
    });
  }
}
