import { NgModule } from '@angular/core';
import { Routes, RouterModule, PreloadAllModules, NoPreloading } from '@angular/router';
import { LoginComponent } from './componet/login/login.component';
import { AuthGuardService } from './services/auth-guard.service';
import { HomeComponent } from './componet/home/home.component';
import { ForgotPasswordComponent } from './componet/forgot-password/forgot-password.component';

const routes: Routes = [

    { path: "home", component: HomeComponent },
    { path: "login", component: LoginComponent },
    { path: 'forgotPassword', component: ForgotPasswordComponent },
    { path: "qbuser", loadChildren: './componet/qbUserM/qbUser.module#QbUserModule' },
    { path: "user", loadChildren: './componet/userManagerM/user.module#UserModule' },
    { path: "saleorder", loadChildren: './componet/saleOrderM/saleOrder.module#SaleOrderMModule' },
    { path: "**", component: HomeComponent }
];

@NgModule({
    imports: [RouterModule.forRoot(routes, { preloadingStrategy:  PreloadAllModules })],
    exports: [RouterModule]
})
export class AppRoutingModule { }
