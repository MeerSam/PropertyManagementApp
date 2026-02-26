import { Routes } from '@angular/router';
import { Home } from '../features/home/home';
import { MemberList } from '../features/members/member-list/member-list';
import { MemberDetailed } from '../features/members/member-detailed/member-detailed';
import { Messages } from '../features/messages/messages';
import { Dashboard } from '../features/dashboard/dashboard';
import { authGuard } from '../core/guards/auth-guard';
import { TestErrors } from '../features/test-errors/test-errors';
import { NotFound } from '../shared/errors/not-found/not-found';
import { ServerError } from '../shared/errors/server-error/server-error';

export const routes: Routes = [
    { path: '', component: Home },
    {
        path: '',
        canActivate: [authGuard],
        runGuardsAndResolvers: 'always',
        children: [
            { path: 'members', component: MemberList },
            { path: 'members/:id', component: MemberDetailed },
            { path: 'messages', component: Messages },
            { path: 'dashboard', component: Dashboard },
        ] 
    },
    { path: 'errors', component: TestErrors },
    { path: 'server-error', component: ServerError },
    { path: '**', component: NotFound }
];
