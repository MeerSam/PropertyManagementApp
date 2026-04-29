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
import { memberResolver } from '../features/members/member-resolver';
import { MemberProfile } from '../features/members/member-profile/member-profile';
import { preventUnsavedChangesGuard } from '../core/guards/prevent-unsaved-changes-guard'; 
import { DocumentList } from '../features/documents/document-list/document-list';

export const routes: Routes = [
    { path: '', component: Home },
    {
        path: '',
        canActivate: [authGuard],
        runGuardsAndResolvers: 'always',
        children: [
            { path: 'members', 
                canActivate: [authGuard],
                runGuardsAndResolvers: 'always', 
                children: [
                    { path: '', component: MemberList, pathMatch: 'full' },
                    { path: ':id', 
                        component: MemberDetailed, 
                        runGuardsAndResolvers: 'always',
                        resolve: {member: memberResolver},
                        title: 'Details', 
                        children: [
                            { path: '', redirectTo:'profile',  pathMatch:'full'},                           
                            { path: 'profile', 
                                component: MemberProfile,  
                                title:'Profile', 
                                canDeactivate:[preventUnsavedChangesGuard], 
                                runGuardsAndResolvers: 'always',
                            },
                            { path: 'properties', 
                                component: PropertyList,  
                                title:'Properties',
                                canDeactivate:[preventUnsavedChangesGuard], 
                                runGuardsAndResolvers: 'always'},
                            { path: 'documents', 
                                component: MemberProfile,  
                                title:'Properties',
                                canDeactivate:[preventUnsavedChangesGuard], 
                                runGuardsAndResolvers: 'always'}
                        ]
                    },
                ]
            },            
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
                            { path: 'documents', component: PropertyDocuments, title:'Documents'},
                        ]
                    },
                ]
            },
            {
                path: 'documents',
                canActivate:[authGuard],
                runGuardsAndResolvers:'always',
                component: DocumentList
            }
        ]
    },
    { path: 'errors', component: TestErrors },
    { path: 'server-error', component: ServerError },
    { path: '**', component: NotFound }
];

// ### 6. **Recommended Route Structure**
// ```
// /dashboard                    -> redirects based on role
// /dashboard/admin/*            -> Admin views
// /dashboard/board/*            -> Board member views
// /dashboard/manager/*          -> Property manager views
// /dashboard/owner/*            -> Owner views (Primary/Resident gated at component level)
// ```
