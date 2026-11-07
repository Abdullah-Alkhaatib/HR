import { Component } from '@angular/core';
import { FormGroup, FormControl, Validators, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
  imports: [ReactiveFormsModule],
  selector: 'app-login',
  styleUrl: './login.css',
  templateUrl: './login.html',
})
export class Login {

  constructor(private _authService: AuthService) { }

  loginForm: FormGroup = new FormGroup({
    Username: new FormControl(null, [Validators.required]),
    Password: new FormControl(null, [Validators.required])
  });

  login() {
    let loginObj = {
      UserName: this.loginForm.value.Username,
      Password: this.loginForm.value.Password,
    };

    this._authService.login(loginObj).subscribe({
      next: (res: any) => {
        // console.log(res); // token
        localStorage.setItem('token', res.token);
      },
      error: (err) => {
        console.log(err.error.message ?? err.error ?? "Unexpected Error");
      }
    });
  }
}
