import { Component, computed, inject, input, model, OnInit, output, signal, ViewChild } from '@angular/core';
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
import { JsonPipe } from '@angular/common';


@Component({
  selector: 'app-document-upload',
  imports: [FileUpload, ReactiveFormsModule, TextInput, SelectInput, JsonPipe],
  templateUrl: './document-upload.html',
  styleUrl: './document-upload.css',
})
export class DocumentUpload implements OnInit {
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
    const role = this.session.currentUser()?.role;
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
    // console.log('scope=', scope, " | this.scopeValue=", this.scopeValue());
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

      return ownersList
    }
    return [];
  })

  protected propertyOption = computed<SelectOption[]>(() => {
    const scope = this.documentForm.get('scope')?.value;
    if (scope === 'OwnerTenure' || scope === 'PropertyHistory') {
      if (this.property() || this.propertyId()) {
        const prop = this.property();
        if (prop) {
          return [{
            value: prop.id,
            label: `${prop.address ? prop.address : prop.id}${prop.unit ? ' #' + prop.unit : ''}`

          }]

        } else if (this.propertyId()) {
          return [{
            value: this.propertyId(),
            label: this.propertyId()
          }]
        }

      } else {
        if (this.ownerships().length > 0) {
          const propertyList: SelectOption[] = this.ownerships()
            .filter(o => o.endDate === null && o.property !== null)
            .map(o => ({
              value: o.property?.id ?? "",
              label: `${o.property?.address ?? o.property?.address} # ${o.property?.unit ?? o.property?.unit} (${o.ownershipType})`
            }));
          // console.log(propertyList);
          return propertyList;
        } 
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
      propertyOwnershipId: ['', [this.optionSelected('scope')]]
    }
    );

    // Adding a listener in the constructor
    this.documentForm.get('scope')?.valueChanges.subscribe(val => {
      console.log(`Changed scope: ${val}`)
      this.scopeValue.set(val ?? '');

      this.documentForm.get('propertyOwnershipId')?.clearValidators();
      this.documentForm.get('propertyId')?.clearValidators();

      if (this.scopeValue() === 'PropertyHistory') {
        this.documentForm.get('propertyId')?.setValidators([Validators.required]);
        this.documentForm.get('propertyOwnershipId')?.clearValidators(); 
      }

      if (this.scopeValue() === 'OwnerTenure') {
        this.documentForm.get('propertyId')?.setValidators([Validators.required]);
        this.documentForm.get('propertyOwnershipId')?.setValidators([Validators.required]); 
      } 
      this.documentForm.controls['propertyId'].updateValueAndValidity();
      this.documentForm.controls['propertyOwnershipId'].updateValueAndValidity();
    });

  }

  ngOnInit() {

    console.log('propertyId=',this.propertyId());
    console.log('ownershipId=',this.ownershipId());
    console.log('property=',this.property());
    console.log('ownerships=',this.ownerships()); 

    if (this.scopeValue() === 'PropertyHistory') {
      this.documentForm.get('propertyId')?.setValidators([Validators.required]);
      this.documentForm.get('propertyOwnershipId')?.clearValidators();
      this.documentForm.get('propertyOwnershipId')?.updateValueAndValidity();
    }

    if (this.scopeValue() === 'OwnerTenure') {
      this.documentForm.get('propertyId')?.setValidators([Validators.required]);
      this.documentForm.get('propertyOwnershipId')?.setValidators([Validators.required]);
    }
  }

  onUploadFile(file: File) {
    this.loading.set(true);
    this.uploadedFile.set(file);
    this.documentForm.controls['propertyId'].updateValueAndValidity();
    this.documentForm.controls['propertyOwnershipId'].updateValueAndValidity();

    console.log("onUploadFile=", this.documentForm.valid);
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
        propertyOwnershipId: this.resolvedOwnershipId()
      }
      this.uploadService.uploadDocument(uploadedFile, this.docInfo).subscribe({
        next: result => {
          this.loading.set(false);
          this.resetForm();
          this.newDocument.emit(result);
        },
        error: error => {
          console.error(`Could not create Document (${this.uploadedFile()?.size}) or form is invalid (${this.documentForm.valid})`);
          console.error(error.error);
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

      if (!this.documentForm.get('propertyOwnershipId')?.value && !this.ownershipId()) {
        this.toastService.error("Ownership selection is required and cannot be null")
      }
      return this.documentForm.get('propertyOwnershipId')?.value ?? this.ownershipId()
    }
    return "";
  }

  cancel() { 
    this.resetForm(); 
    this.cancelCreate.emit(false);
  }

  private resolvedPropertyId(): string {

    const scope = this.documentForm.get('scope')?.value;

    if (scope === 'OwnerTenure' || scope === 'PropertyHistory') {
      if (this.propertyId() !='' && this.documentForm.get('propertyId')?.value !== this.propertyId()) {
        // console.log(this.documentForm.get('propertyId')?.value, this.propertyId)
        // console.log("Property selected in the form is different from input", this.property()?.id)

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
      console.log(choice, choiceValue);
      if (choice === 'scope' && choiceValue === 'OwnerTenure') {
        console.log("If choiceValue === 'OwnerTenure'", choiceValue, control.value);
        return control.value ? null : { ownershipNotSelected: true };
      } else if (choice === 'scope' && choiceValue === 'PropertyHistory') {
        console.log("If choiceValue === 'PropertyHistory'", choiceValue, control.value);
        return control.value ? null : { propertyNotSelected: true };
      }
      return null;
    };
  }
}
