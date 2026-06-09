import { Component, inject, OnInit, signal } from '@angular/core';
import { SessionService } from '../../../../core/services/session-service';
import { ActivatedRoute, Router } from '@angular/router';
import { APP_ROLE_LABELS, EditableUser, Role, User } from '../../../../types/user';
import { ToastService } from '../../../../core/services/toast-service';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { TextInput } from "../../../../shared/text-input/text-input";
import { SelectInput } from "../../../../shared/select-input/select-input";

@Component({
  selector: 'app-user-profile',
  imports: [FormsModule, TextInput, ReactiveFormsModule, SelectInput],
  templateUrl: './user-profile.html',
  styleUrl: './user-profile.css',
})
export class UserProfile implements OnInit {

  protected session = inject(SessionService)
  protected route = inject(ActivatedRoute)
  protected user = signal<User | null>(null);
  protected toast = inject(ToastService)
  protected userEditForm: FormGroup = new FormGroup({}) 
  private router = inject(Router);

  protected editableUser: EditableUser = {
    userId: '',
    displayName: '',
    firstName: '',
    lastName: '',
    email: '',
    role: 'resident',
    dateOfBirth: '',
    gender: ''
  }

  ngOnInit(): void {
    this.route.parent?.data.subscribe(data => {
      if (data) {
        this.user.set(data['user']);
      } else {
        this.toast.error('Error loading ember from parent')
      }
    });

    this.initializeForm();
  }

  initializeForm() {
    this.userEditForm = new FormGroup({
      userId: new FormControl(this.user()?.id || '', Validators.required),
      firstName: new FormControl(this.user()?.firstName || '', Validators.required),
      lastName: new FormControl(this.user()?.lastName || '', Validators.required),
      email: new FormControl(this.user()?.email || '', Validators.required),
      displayName: new FormControl(this.user()?.displayName || '', Validators.required),
      role: new FormControl(this.user()?.role || null, Validators.required),
      gender: new FormControl(this.user()?.gender || '', Validators.required),
      dateOfBirth: new FormControl(this.user()?.dateOfBirth || '', Validators.required),
    })
  }
  ngOnDestroy(): void {
    if (this.session.editMode()) {
      // console.log('resetting editmode to false... current value', this.session.editMode())
      this.session.editMode.set(false);

    }
  }
  getRole(role: Role) {
    return role ? APP_ROLE_LABELS[role] ?? 'Unknown Role' : 'Unknown Role';
  }

  updateProfile() {
    if (this.userEditForm.valid) {
      const formData = { ...this.userEditForm.value };
      // console.log(formData)
      // this.session.updateUser(formData).subscribe({
      //   next: () => {
      //     this.toast.success('Profile Updated for user succesfully');
      //     this.session.editMode.set(false);
      //     this.user.set(formData as User);
      //     if (formData.userId == this.session.currentUser()?.id &&
      //      formData.displayName !== this.session.currentUser()?.displayName  ) {
      //       this.session.currentUser.update(u => ({ ...u!, displayName: formData.displayName }));
      //     }
      //   },
      //   error: error => this.toast.error('Error while saving.' + error)
      // });

    }
  }
  getMaxDate() {
    const today = new Date();
    today.setFullYear(today.getFullYear() - 18);
    return today.toISOString().split('T')[0];
  }
}
