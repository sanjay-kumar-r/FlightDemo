import { Injectable, ComponentFactoryResolver, ViewContainerRef } from '@angular/core';
import { from, Observable } from 'rxjs';
import { map } from 'rxjs/operators';

export interface ComponentLoader {
  loadChildren: () => Promise<any>;
}

@Injectable({
  providedIn: 'root'
})
export class DynamicComponentService {

  constructor(private cfr: ComponentFactoryResolver) {}

  loadComponents(vcr: ViewContainerRef, cl: ComponentLoader, data:any) {
    return from(cl.loadChildren()).pipe(
      map((component: any) => this.cfr.resolveComponentFactory(component)),
      map(componentFactory => vcr.createComponent(componentFactory))
      // ,
      // map((componentRef:any) => componentRef.instance.data = data)
    );
  }
}
