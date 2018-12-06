import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

@Injectable({
    providedIn: 'root'
  })
export class PreventUnsavedChanges implements CanDeactivate<MemberEditComponent> {

    canDeactivate(component: MemberEditComponent): boolean {
        // viewChild decorator gives us access to editForm from memberEditComponent
        if (component.editForm.dirty) {
            return confirm('Are you sure you want to leave! Any unsaved changes will be lost!');
        }

        return true;
    }

}
