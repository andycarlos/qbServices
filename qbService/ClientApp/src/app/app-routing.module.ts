import { NgModule } from '@angular/core';
import { Routes, RouterModule, PreloadAllModules, NoPreloading } from '@angular/router';
import { LoginComponent } from './componet/login/login.component';
import { AuthGuardService } from './services/auth-guard.service';
import { HomeComponent } from './componet/home/home.component';

const routes: Routes = [

    { path: "home", component: HomeComponent },
    { path: "login", component: LoginComponent },
    { path: "user", loadChildren: './componet/user-manager/user.module#UserModule' },
    { path: "**", component: HomeComponent }
];

@NgModule({
    imports: [RouterModule.forRoot(routes, { preloadingStrategy:  PreloadAllModules })],
    exports: [RouterModule]
})
export class AppRoutingModule { }
