import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login').then(m => m.LoginComponent)
  },
  {
    path: '',
    loadComponent: () => import('./layout/shell/shell').then(m => m.ShellComponent),
    canActivate: [authGuard],
    children: [
      {
        path: '',
        redirectTo: 'issues',
        pathMatch: 'full'
      },
      {
        path: 'projects',
        loadComponent: () => import('./features/projects/project-list/project-list').then(m => m.ProjectListComponent)
      },
      {
        path: 'issues',
        loadComponent: () => import('./features/issues/issue-list/issue-list').then(m => m.IssueListComponent)
      },
      {
        path: 'issues/:id',
        loadComponent: () => import('./features/issues/issue-detail/issue-detail').then(m => m.IssueDetailComponent)
      },
      {
        path: 'reports',
        loadComponent: () => import('./features/reports/report-dashboard/report-dashboard').then(m => m.ReportDashboardComponent)
      }
    ]
  },
  {
    path: '**',
    redirectTo: ''
  }
];
