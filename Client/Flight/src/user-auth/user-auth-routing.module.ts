import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RegisterComponent } from './register/register.component';
import { LoginComponent } from './login/login.component';

// const routes: Routes = [
//   {
//     path:"users",
//     children:[
//       {
//         path:"login",
//         component:LoginComponent
//       },
//       {
//         path:"register",
//         component:RegisterComponent
//       },
//       {
//         path:"",
//         redirectTo:"login",
//         pathMatch:"full"
//       }
//     ]
//   },
//   {
//     path:"",
//     redirectTo:"users",
//     pathMatch:"full"
//   }
// ]

const routes: Routes = [
  {
    path:"login",
    component:LoginComponent
  },
  {
    path:"register",
    component:RegisterComponent
  },
  {
    path:"",
    redirectTo:"login",
    pathMatch:"full"
  }
]


@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UserAuthRoutingModule { }
