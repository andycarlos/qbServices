import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
//-------------
//import { UserModule } from './componet/user-manager/user.module';
//----------------
import { AppComponent } from './app.component';
import { LoginComponent } from './componet/login/login.component';
import { NavbarComponent } from './componet/navbar/navbar.component';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { InterceptorHttpService } from './services/interceptor-http.service';
import { HomeComponent } from './componet/home/home.component';
import { PruebaComponent } from './componet/prueba/prueba.component';
//ng bootstrap
//import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
@NgModule({
    declarations: [
        AppComponent,
        LoginComponent,
        NavbarComponent,
        HomeComponent,
        PruebaComponent,
    ],
    imports: [
        BrowserModule,
        //UserModule,
        HttpClientModule,
        ReactiveFormsModule,
        FormsModule,
        AppRoutingModule,
       // NgbModule,
    ],
    providers: [
        { provide: HTTP_INTERCEPTORS, useClass: InterceptorHttpService, multi: true }],
    bootstrap: [AppComponent]
})
export class AppModule { }
