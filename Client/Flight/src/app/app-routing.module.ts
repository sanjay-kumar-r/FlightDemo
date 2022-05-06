import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path:"users",
    loadChildren:() => import('../user-auth/user-auth.module').then(m => m.UserAuthModule)
  },
  {
    path:"home",
    loadChildren:() => import('../home/home.module').then(m => m.HomeModule)
  },
  {
    path:"",
    redirectTo:"users",
    pathMatch:"full"
  }
  // ,
  // {
  //   path:"admin",
  //   loadChildren:() => import('../user-auth/user-auth.module').then(m => m.UserAuthModule)
  // }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
