import { Component, computed, inject, input, model, output, signal, ViewChild } from '@angular/core';
import { FileUpload } from "../file-upload/file-upload";
import { UploadService } from '../../core/services/upload-service';
import { SessionService } from '../../core/services/session-service';
import { DocumentUploadRequest, DOCUMENT_SCOPES, DOCUMENT_SCOPE_LABELS, Document, ownerTenureOptions, propertyHistoryOptions } from '../../types/document';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { ToastService } from '../../core/services/toast-service';
import { TextInput } from "../text-input/text-input";
import { SelectOption } from '../../types/select';
import { communityOptions, } from '../../types/document';
import { SelectInput } from "../select-input/select-input";
import { Property, PropertyOwnership } from '../../types/property';
import { ValidationError } from '@angular/forms/signals';


@Component({
  selector: 'app-document-upload',
  imports: [FileUpload, ReactiveFormsModule, TextInput, SelectInput],
  templateUrl: './document-upload.html',
  styleUrl: './document-upload.css',
})
export class DocumentUpload {
  @ViewChild('fileUpload') fileUploadRef!: FileUpload;
  private uploadService = inject(UploadService);
  protected session = inject(SessionService);
  protected toastService = inject(ToastService)
  private fb = inject(FormBuilder);
  propertyId = input<string>('');
  property = input<Property | null>(null);
  ownershipId = input<string>('');
  ownerships = input<PropertyOwnership[]>([]);
  protected loading = model<boolean>(false);
  protected cancelCreate = output<boolean>();
  protected newDocument = output<Document>();
  protected scopes = DOCUMENT_SCOPES;
  protected scopeLabels = DOCUMENT_SCOPE_LABELS;
  protected docInfo = {} as DocumentUploadRequest;
  protected uploadedFile = signal<File | null>(null);
  protected documentForm: FormGroup = new FormGroup({});
  protected scopeValue = signal<string>('');



  // Scope options are filtered by role
  protected scopeOptions = computed<SelectOption[]>(() => {
    const role = this.session.currentUser()?.appRole;
    const adminRoles = ['admin', 'board_member', 'property_manager'];
    const limitedRoles = ['owner', 'resident'];

    if (adminRoles.includes(role ?? '')) {
      return DOCUMENT_SCOPES.map(s => ({
        value: s, label: DOCUMENT_SCOPE_LABELS[s]
      }))
    }
    if (limitedRoles.includes(role ?? '')) {
      return DOCUMENT_SCOPES
        .filter(s => s === 'PropertyHistory' || s === 'OwnerTenure')
        .map(s => ({ value: s, label: DOCUMENT_SCOPE_LABELS[s] }))
    }
    return [];
  });


  // Category options react to the scope value
  protected categoryOptions = computed<SelectOption[]>(() => {
    const scope = this.documentForm.get('scope')?.value;
    //console.log('scope=', scope, " | this.scopeValue=", this.scopeValue());
    //console.log(this.documentForm);

    const map: Record<string, SelectOption[]> = {
      Public: communityOptions,
      Community: communityOptions,
      OwnerTenure: ownerTenureOptions,
      PropertyHistory: propertyHistoryOptions,
    };
    return map[this.scopeValue()] ?? [];
  });

  protected ownershipOptions = computed<SelectOption[]>(() => {
    const scope = this.documentForm.get('scope')?.value;

    if (scope === 'OwnerTenure') {
      const ownersList: SelectOption[] = this.ownerships().map(o => ({
        value: o.id,
        label: `${o.member?.displayName ?? o.memberId} (${o.ownershipType})`
      }));
      // console.log(this.ownerships());
      return ownersList
    }
    return [];
  })

  protected propertyOption = computed<SelectOption[]>(() => {
    const scope = this.documentForm.get('scope')?.value;

    if (scope === 'OwnerTenure' || scope === 'PropertyHistory') {
      const prop = this.property();
      if (prop) {
        return [{
          value: prop.id,
          label: prop.unit
        }]

      } else if (this.propertyId()) {
        return [{
          value: this.propertyId(),
          label: this.propertyId()
        }]
      }
    }
    return [];
  });


  constructor() {
    this.documentForm = this.fb.group({
      title: ['', [Validators.required]],
      description: [''],
      scope: ['', [Validators.required]],
      category: ['', [Validators.required]],
      notes: [],
      propertyId: ['', [this.optionSelected('scope')]],
      ownershipId: ['', [this.optionSelected('scope')]]
    }
    );

    // Adding a listener in the constructor
    this.documentForm.get('scope')?.valueChanges.subscribe(val => {
      // console.log(`Changed scope: ${val}`)
      this.scopeValue.set(val ?? '');
      this.documentForm.get('category')?.reset('');
      this.documentForm.get('propertyId')?.reset('');
      this.documentForm.get('ownershipId')?.reset('');
      this.documentForm.controls['propertyId'].updateValueAndValidity();
      this.documentForm.controls['ownershipId'].updateValueAndValidity();
    });

  }

  onUploadFile(file: File) {
    this.loading.set(true);
    this.uploadedFile.set(file);
    if (this.uploadedFile() && this.documentForm.valid) {
      this.createDocument();
    } else {
      this.loading.set(false);
    }
  }

  createDocument() {
    const uploadedFile = this.uploadedFile();

    if (uploadedFile && this.documentForm.valid) {

      this.docInfo = {
        ...this.documentForm.value,
        propertyId: this.resolvedPropertyId(),
        ownershipId: this.resolvedOwnershipId()
      }
      this.uploadService.uploadDocument(uploadedFile, this.docInfo).subscribe({
        next: result => {
          this.loading.set(false);
          this.resetForm();
          this.newDocument.emit(result);
        },
        error: err => {
          console.error(err);
          this.loading.set(false);
        }
      });
    } else {
      this.loading.set(false);
      this.toastService.error(`Could not create Document (${this.uploadedFile()?.size}) or form is invalid (${this.documentForm.valid})`);
      this.toastService.error(`Missing Attachment (${this.uploadedFile()?.size}) or form is invalid (${this.documentForm.valid})`);
    }
    this.cancel();
  }
  private resolvedOwnershipId() {
    const scope = this.documentForm.get('scope')?.value;

    if (scope === 'OwnerTenure') {

      if (!this.documentForm.get('ownershipId')?.value && !this.ownershipId()) {
        this.toastService.error("Ownership selection is required and cannot be null")
      }
      return this.documentForm.get('ownershipId')?.value ?? this.ownershipId()
    }
    return "";
  }

  cancel() {
    // console.log('reseting form : docUpload.cancel()')
    this.resetForm();

    this.cancelCreate.emit(false);
  }

  private resolvedPropertyId(): string {

    const scope = this.documentForm.get('scope')?.value;

    if (scope === 'OwnerTenure' || scope === 'PropertyHistory') {
      if (this.documentForm.get('propertyId')?.value !== this.propertyId) {
        this.toastService.error("Property selected in the form is different from input")
      }
      if (!this.documentForm.get('propertyId')?.value && !this.propertyId()) {
        this.toastService.error("Property cannot be null")
      }
      return this.documentForm.get('propertyId')?.value ?? this.propertyId()
    }
    return "";
  }

  resetForm() {
    this.documentForm.reset();
    this.scopeValue.set('');
    this.uploadedFile.set(null);
    this.loading.set(false)
    this.docInfo = {} as DocumentUploadRequest;
    // console.log(this.fileUploadRef); 
    this.fileUploadRef?.reset();
  }

  private optionSelected(choice: string): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const parent = control.parent; //control.parent  is the formGroup

      if (!parent) return null;
      const choiceValue = parent.get(choice)?.value;
      if (choice === 'scope' && choiceValue === 'OwnerTenure') {
        return control.value ? null : { ownershipNotSelected: true };
      } else if (choice === 'scope' && choiceValue === 'PropertyHistory') {
        return control.value ? null : { propertyNotSelected: true };
      }
      return null;
    };
  }
}
