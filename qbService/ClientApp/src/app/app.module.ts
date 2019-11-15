import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
//-------------
//import { UserModule } from './componet/user-manager/user.module';
//----------------
import { AppComponent } from './app.component';
import { LoginComponent } from './componet/login/login.component';
import { NavbarComponent } from './componet/navbar/navbar.component';
import { HttpClientModule } from '@angular/common/http';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ReactiveFormsModule } from '@angular/forms';

@NgModule({
    declarations: [
        AppComponent,
        LoginComponent,
        NavbarComponent,
    ],
    imports: [
        BrowserModule,
        //UserModule,
        AppRoutingModule,
        HttpClientModule,
        NgbModule,
        ReactiveFormsModule
    ],
    providers: [],
    bootstrap: [AppComponent]
})
export class AppModule { }
