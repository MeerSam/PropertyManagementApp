import { Routes } from '@angular/router';
import { Home } from '../features/home/home';
import { MemberList } from '../features/members/member-list/member-list';
import { MemberDetailed } from '../features/members/member-detailed/member-detailed';
import { Messages } from '../features/messages/messages';
import { Dashboard } from '../features/dashboards/dashboard';
import { authGuard } from '../core/guards/auth-guard';
import { TestErrors } from '../features/test-errors/test-errors';
import { NotFound } from '../shared/errors/not-found/not-found';
import { ServerError } from '../shared/errors/server-error/server-error';
import { PropertyList } from '../features/properties/property-list/property-list';
import { PropertyDetails } from '../features/properties/property-details/property-details';
import { DashboardAdmin } from '../features/dashboards/dashboard-admin/dashboard-admin';
import { PropertyProfile } from '../features/properties/property-profile/property-profile';
import { PropertyDocuments } from '../features/properties/property-documents/property-documents';
import { propertyResolver } from '../features/properties/property-resolver';

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
            {
                path: 'dashboard',
                canActivate: [authGuard],
                runGuardsAndResolvers: 'always',
                component: Dashboard,
                children: [
                    { path: 'admin', component: DashboardAdmin },
                    { path: 'board', component: Dashboard },
                    { path: 'member', component: Dashboard },
                    { path: 'manager', component: Dashboard },
                ]
            },
            {
                path: 'properties',
                canActivate: [authGuard],
                runGuardsAndResolvers: 'always',
                children: [
                    { path: '', component: PropertyList, pathMatch: 'full' },
                    {
                        path: ':id', 
                        component: PropertyProfile,  
                        resolve:  {property: propertyResolver},
                        runGuardsAndResolvers: 'always',
                        title:'Profile',
                        children: [
                            { path: '', redirectTo:'details', pathMatch:'full'},
                            { path: 'details', component: PropertyDetails,  title:'Details'},
                            { path: 'documents', component: PropertyDocuments, title:'Documents' },
                        ]
                    },
                ]
            },
        ]
    },
    { path: 'errors', component: TestErrors },
    { path: 'server-error', component: ServerError },
    { path: '**', component: NotFound }
];

// ### 6. **Recommended Route Structure**
// ```
// /dashboard                    → redirects based on role
// /dashboard/admin/*            → Admin views
// /dashboard/board/*            → Board member views
// /dashboard/manager/*          → Property manager views
// /dashboard/owner/*            → Owner views (Primary/Resident gated at component level)
// ```
