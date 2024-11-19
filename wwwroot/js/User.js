function login() {
  if (!validateLoginForm()) {
    return;
  }

  var email = $("#Email").val();
  var password = $("#Password").val();
  var rememberMe = $("#RememberMe").prop("checked");
  var returnUrl = "";
  $.ajax({
    url: Url,
    type: "POST",
    data: {
      Email: email,
      Password: password,
      RememberMe: rememberMe,
      ReturnUrl: returnUrl,
    },
    success: function (response) {
      if (response.success) {
        window.location.href = response.redirectUrl;
      } else {
        toastr.error(response.message);
      }
    },
    error: function (xhr, status, error) {
      toastr.error("An error occurred: " + error);
    },
  });
}

function register() {
  if (!validateRegisterForm()) {
    return;
  }

  var email = $("#Email").val();
  var password = $("#Password").val();
  var confirmPassword = $("#ConfirmPassword").val();


  $.ajax({
    url: Url,
    type: "POST",
    data: {
      Email: email,
      Password: password,
      ConfirmPassword: confirmPassword
    },
    success: function (response) {
      if (response.success) {
        toastr.info(response.message);
        window.location.href = response.redirectUrl;
      } else {
        toastr.error(response.message);
      }
    },
    error: function (xhr, status, error) {
      toastr.error("An error occurred: " + error);
    },
  });
}

function ResetPassword(){

  if(!validateResetPasswordForm()){
    return;
  }

  var password = $("#Password").val();
  var confirmPassword = $("#ConfirmPassword").val();

  $.ajax({
    url: Url,
    type: "POST",
    data: {
      Password: password,
      ConfirmPassword: confirmPassword
    },
    success: function (response) {
      if (response.success) {
        window.location.href = response.redirectUrl;
      } else {
        toastr.error(response.message);
      }
    },
    error: function (xhr, status, error) {
      toastr.error("An error occurred: " + error);
    },
  });
  
}

function VerifyOTP(){
  if(!validateOTPForm()){
    return;
  }

  var otp = $("#OTP").val();

  $.ajax({
    url: Url,
    type: "POST",
    data: {
      OTP: otp
    },
    success: function (response) {
      if (response.success) {
        window.location.href = response.redirectUrl;
      } else {
        toastr.error(response.message);
      }
    },
    error: function (xhr, status, error) {
      toastr.error("An error occurred: " + error);
    },
  });
}

function SendEmail(){
  if(!validateEmailForm()){
    return;
  }

  var email = $("#Email").val();

  $.ajax({
    url: Url,
    type: "POST",
    data: {
      Email: email
    },
    success: function (response) {
      if (response.success) {
        window.location.href = response.redirectUrl;
      } else {
        toastr.error(response.message);
      }
    }
  })
}

function validateLoginForm() {
  var email = document.getElementById("Email").value.trim();
  var password = document.getElementById("Password").value;
  var valid = true;

  if (!email) {
    document.getElementById("emailError").textContent =
      "Please enter your email.";
    valid = false;
  } else if (!validateEmail(email)) {
    document.getElementById("emailError").textContent =
      "Please enter a valid email address.";
    valid = false;
  }

  if (!password) {
    document.getElementById("passwordError").textContent =
      "Please enter your password.";
    valid = false;
  }

  return valid;
}

function validateRegisterForm() {
  var email = document.getElementById("Email").value.trim();
  var password = document.getElementById("Password").value;
  var confirmPassword = document.getElementById("ConfirmPassword").value;
  var passwordValidation = validatePassword(password);
  var valid = true;

  if (!email) {
    document.getElementById("emailError").textContent = "Please enter your email.";
    valid = false;
  } else if (!validateEmail(email)) {
    document.getElementById("emailError").textContent = "Please enter a valid email address.";
    valid = false;
  }

  if (!password) {
    document.getElementById("passwordError").textContent = "Please enter your password.";
    valid = false;
  } else if(!passwordValidation.success) {
    document.getElementById("passwordError").textContent = passwordValidation.message;
    valid = false;
  }

  if (!confirmPassword) {
    document.getElementById("confirmPasswordError").textContent = "Please confirm your password.";
    valid = false;
  } else if (password !== confirmPassword) {
    document.getElementById("confirmPasswordError").textContent = "Confirmed password does not match.";
    valid = false;
  }

  return valid;

}

function validResetPasswordForm() {
  var password = document.getElementById("Password").value;
  var confirmPassword = document.getElementById("ConfirmPassword").value;
  var passwordValidation = validatePassword(password);  
  var valid = true;

  if (!email) {
    document.getElementById("emailError").textContent = "Please enter your email.";
    valid = false;
  } else if (!validateEmail(email)) {
    document.getElementById("emailError").textContent = "Please enter a valid email address.";
    valid = false;
  }

  if (!password) {
    document.getElementById("passwordError").textContent = "Please enter your password.";
    valid = false;
  } else if(!passwordValidation.success) {
    document.getElementById("passwordError").textContent = passwordValidation.message;
    valid = false;
  }

  if (!confirmPassword) {
    document.getElementById("confirmPasswordError").textContent = "Please confirm your password.";
    valid = false;
  } else if (password !== confirmPassword) {
    document.getElementById("confirmPasswordError").textContent = "Confirmed password does not match.";
    valid = false;
  }

  return valid;
}

function validateEmailForm() {
  var email = document.getElementById("Email").value.trim();
  var valid = true;

  if (!email) {
    document.getElementById("emailError").textContent = "Please enter your email.";
    valid = false;
  } else if (!validateEmail(email)) {
    document.getElementById("emailError").textContent = "Please enter a valid email address.";
    valid = false;
  }

  return valid;
}

function validatePassword(password) {

  var minLength = 8;
  var maxLength = 20;
  var hasUpperCase = /[A-Z]/;
  var hasLowerCase = /[a-z]/;
  var hasNumber = /\d/;
  var hasSpecialChar = /[!@#$%^&*(),.?":{}|<>]/;
  var messages = [];

  if (password.length < minLength || password.length > maxLength) {
    messages.push(`be ${minLength}-${maxLength} characters long`);
  }
  if (!hasUpperCase.test(password)) {
    messages.push("contain at least one uppercase letter");
  }
  if (!hasLowerCase.test(password)) {
    messages.push("contain at least one lowercase letter");
  }
  if (!hasNumber.test(password)) {
    messages.push("contain at least one number");
  }
  if (!hasSpecialChar.test(password)) {
    messages.push("contain at least one special character");
  }

  if (messages.length > 0) {
    return { success: false, message: `Password must ${messages.join(", ")}.` };
  }
  return { success: true, message: null };
}

function validateEmail(email) {
  var regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return regex.test(email);
}

function validateOTPForm() {
  var otp = document.getElementById("OTP").value;
  var valid = true;

  if (!otp) { 
    document.getElementById("OTPError").textContent = "Please enter your OTP.";
    valid = false;
  }

  return valid;
}

document.getElementById("Email").addEventListener("input", function () {
  document.getElementById("emailError").textContent = "";
});

document.getElementById("Password").addEventListener("input", function () {
  document.getElementById("passwordError").textContent = "";
});

document.getElementById("ConfirmPassword").addEventListener("input", function () {
    document.getElementById("confirmPasswordError").textContent = "";
});

document.getElementById("OTP").addEventListener("input", function () {
  document.getElementById("OTPError").textContent = "";
});

