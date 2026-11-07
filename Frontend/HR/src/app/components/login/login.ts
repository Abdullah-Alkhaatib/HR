import { Component } from '@angular/core';
import { FormGroup, FormControl, Validators, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { ToastrService } from '@relynn/ngx-toastr';
import { Router } from '@angular/router';

@Component({
  imports: [ReactiveFormsModule],
  selector: 'app-login',
  styleUrl: './login.css',
  templateUrl: './login.html',
})
export class Login {

  showErrorMessage: boolean = false;
  errorMessage: string = '';

  constructor(private _authService: AuthService,
    private _toastr: ToastrService,
    private _router: Router
  ) { }

  loginForm: FormGroup = new FormGroup({
    Username: new FormControl(null, [Validators.required]),
    Password: new FormControl(null, [Validators.required])
  });

  login() {
    this.showErrorMessage = false;
    this.errorMessage = '';

    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    let loginObj = {
      UserName: this.loginForm.value.Username,
      Password: this.loginForm.value.Password,
    };

    this._authService.login(loginObj).subscribe({
      next: (res: any) => {
        // console.log(res); // token
        localStorage.setItem('token', res.token);
        localStorage.setItem('role', res.role);
        this.showErrorMessage = false;
        this._router.navigate(['/']);
      },
      error: (err) => {
        this.showErrorMessage = true;
        this.errorMessage = err.error.message ?? err.error ?? "Unexpected Error";
        this._toastr.error(this.errorMessage, 'Login Failed');
      }
    });
  }
}
