import { NgModule } from '@angular/core';
import { Routes, RouterModule, PreloadAllModules } from '@angular/router';
import { LoginComponent } from './componet/login/login.component';

const routes: Routes = [

    { path: "login", component: LoginComponent },
    { path: "user", loadChildren:'./componet/user-manager/user.module#UserModule' },
    { path: "**", component: LoginComponent }
];

@NgModule({
    imports: [RouterModule.forRoot(routes, { preloadingStrategy: PreloadAllModules })],
    exports: [RouterModule]
})
export class AppRoutingModule { }
