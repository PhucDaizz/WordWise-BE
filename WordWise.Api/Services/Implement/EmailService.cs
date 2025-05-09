using System.Net.Mail;
using System.Net;
using WordWise.Api.Models.Domain;
using WordWise.Api.Services.Interface;
using Microsoft.AspNetCore.Identity;

namespace WordWise.Api.Services.Implement
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<ExtendedIdentityUser> userManager;

        public EmailService(IConfiguration configuration, UserManager<ExtendedIdentityUser> userManager)
        {
            this.configuration = configuration;
            this.userManager = userManager;
        }

        public Task SendEmailAsync(string toEmail, string subject, string body, bool isBodyHTML)
        {
            string MailServer = configuration["EmailSettings:MailServer"];
            string FromEmail = configuration["EmailSettings:FromEmail"];
            string Password = configuration["EmailSettings:Password"];
            int Port = int.Parse(configuration["EmailSettings:MailPort"]);

            var smtpClient = new SmtpClient(MailServer, Port)
            {
                Credentials = new NetworkCredential(FromEmail, Password),
                EnableSsl = true
            };

            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(FromEmail),
                To = { toEmail },
                Subject = subject,
                Body = body,
                IsBodyHtml = isBodyHTML
            };
            return smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendEmailConfirmationAsync(ExtendedIdentityUser user)
        {
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(token));
            var confirmationLink = $"http://localhost:3000/verifyemail?userId={user.Id}&token={encodedToken}";

            string htmlBody = GenerateEmailConfirm(user, confirmationLink);

            await SendEmailAsync(user.Email, "Confirm Your WordWise Account", htmlBody, true);
        }

        public async Task SendPasswordResetEmailAsync(string email, string userName, string resetLink)
        {
            string htmlBody = GenarateEmailResetPassword(userName, resetLink);

            await SendEmailAsync(email, "Reset Your WordWise Password", htmlBody, true);
        }


        private string GenerateEmailConfirm(ExtendedIdentityUser user, string confirmationLink)
        {
            return $@"
                    <!DOCTYPE html>
                    <html lang=""en"">
                    <head>
                        <meta charset=""UTF-8"">
                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                        <title>Confirm Your WordWise Account</title>
                        <style>
                            /* Basic Reset */
                            body, table, td, a {{ -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; }}
                            table, td {{ mso-table-lspace: 0pt; mso-table-rspace: 0pt; }}
                            img {{ -ms-interpolation-mode: bicubic; border: 0; height: auto; line-height: 100%; outline: none; text-decoration: none; }}
                            table {{ border-collapse: collapse !important; }}
                            body {{ height: 100% !important; margin: 0 !important; padding: 0 !important; width: 100% !important; background-color: #f7f7f7; /* Slightly lighter grey */ }}

                            /* Main Styles - Engaging & Clean */
                            .container {{
                                padding: 0; /* Remove container padding, handle inside */
                                max-width: 580px;
                                margin: 25px auto;
                                background-color: #ffffff;
                                border-radius: 12px; /* Slightly more rounded */
                                font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif;
                                color: #333333;
                                line-height: 1.65; /* Adjust line height */
                                box-shadow: 0 5px 20px rgba(0,0,0,0.07);
                                overflow: hidden; /* Ensure rounded corners contain content */
                            }}
                            .header {{
                                background-color: #E8F5E9; /* Light Green */
                                padding: 45px 25px 35px 25px; /* Adjust padding */
                                text-align: center;
                            }}
                            .header h1 {{
                                margin: 0;
                                color: #2E7D32; /* Vibrant Green */
                                font-size: 26px; /* Adjust size */
                                font-weight: bold;
                                line-height: 1.3;
                            }}
                             .header .sub-text {{
                                 font-size: 16px;
                                 color: #4CAF50; /* Complementary Green */
                                 margin-top: 8px;
                             }}
                            .content {{
                                padding: 40px 30px; /* More content padding */
                            }}
                            .content p {{
                                margin: 0 0 20px 0;
                                font-size: 16px;
                                color: #444444; /* Slightly darker grey for better readability */
                            }}
                            .content .highlight {{
                                 color: #1E8449; /* Darker Green highlight */
                                 font-weight: bold;
                            }}
                            .button-cta-container {{
                                text-align: center;
                                padding: 10px 0 25px 0; /* Adjust button spacing */
                            }}
                            .button-cta {{
                                display: inline-block;
                                padding: 15px 40px; /* Adjust button padding */
                                background-color: #FF9800; /* Orange */
                                color: #ffffff !important;
                                text-decoration: none;
                                font-weight: bold;
                                border-radius: 30px; /* Very rounded */
                                font-size: 17px;
                                text-align: center;
                                border: none;
                                cursor: pointer;
                                transition: transform 0.1s ease, background-color 0.2s ease;
                            }}
                            .button-cta:hover {{
                                 background-color: #FB8C00;
                                 transform: scale(1.03); /* Slight zoom on hover */
                             }}
                            .link-alternative {{
                                font-size: 13px;
                                color: #777777;
                                margin-top: 25px;
                                text-align: center;
                                word-break: break-all;
                            }}
                             .link-alternative a {{
                                color: #FF9800 !important;
                                text-decoration: underline;
                                font-weight: 500; /* Medium weight */
                             }}
                            .footer {{
                                padding: 30px 25px;
                                text-align: center;
                                font-size: 12px;
                                color: #aaaaaa; /* Lighter footer text */
                                background-color: #fcfcfc; /* Very light grey footer background */
                                border-top: 1px solid #f0f0f0;
                            }}
                             .footer a {{
                                 color: #888888 !important;
                                 text-decoration: none;
                                 font-weight: 500;
                             }}
                             .footer a:hover {{
                                 text-decoration: underline;
                             }}
                            /* Responsive Styles */
                            @media screen and (max-width: 600px) {{
                                body {{ font-size: 16px; }} /* Prevent tiny fonts on mobile */
                                .container {{
                                    width: 100% !important;
                                    margin: 0 !important;
                                    border-radius: 0 !important;
                                    box-shadow: none !important;
                                }}
                                .header {{ padding: 35px 20px 30px 20px; }}
                                .header h1 {{ font-size: 24px; }}
                                .header .sub-text {{ font-size: 15px; }}
                                .content {{ padding: 30px 20px; }}
                                .content p {{ font-size: 15px; margin-bottom: 18px; }}
                                .button-cta {{ padding: 14px 30px; font-size: 16px; }}
                                .footer {{ padding: 25px 20px; font-size: 11px;}}
                            }}
                        </style>
                    </head>
                    <body style=""margin: 0 !important; padding: 0 !important; background-color: #f7f7f7;"">
                        <!-- Outer Table for Background -->
                        <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background-color: #f7f7f7;"">
                            <tr>
                                <td align=""center"" style=""padding: 10px;""> <!-- Add padding around container -->
                                    <!-- Main Content Container -->
                                    <!--[if mso | IE]>
                                    <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""580"" align=""center"" style=""width:580px; background-color:#ffffff; border-radius: 12px;"">
                                    <tr>
                                    <td style=""line-height:0px;font-size:0px;mso-line-height-rule:exactly;"">
                                    <![endif]-->
                                    <div class=""container"" style=""max-width:580px;margin:25px auto;background:#ffffff;border-radius:12px;overflow:hidden;"">
                                        <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" style=""font-size:0px;width:100%;background:#ffffff;border-radius:12px;"" align=""center"" border=""0"">
                                            <tbody>
                                                <tr>
                                                    <td style=""vertical-align:top;padding:0;"">
                                                        <!-- Header Section -->
                                                        <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" style=""width:100%; background-color: #E8F5E9;"">
                                                            <tr>
                                                                <td align=""center"" style=""padding: 45px 25px 35px 25px;"">
                                                                    <!-- Optional Logo -->
                                                                    <!-- <img src=""[Your Logo URL]"" alt=""WordWise Logo"" width=""80"" style=""margin-bottom: 15px; border-radius: 8px;""> -->
                                                                    <h1 style=""margin: 0; color: #2E7D32; font-size: 26px; font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif; font-weight: bold; line-height: 1.3;"">One Click Away From<br>Mastering Words!</h1>
                                                                    <p class=""sub-text"" style=""font-size: 16px; color: #4CAF50; margin-top: 8px; font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif;"">Let's confirm it's you.</p>
                                                                </td>
                                                            </tr>
                                                        </table>

                                                        <!-- Content Section -->
                                                        <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" style=""width:100%;"">
                                                            <tr>
                                                                <td align=""left"" style=""padding: 40px 30px; font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif;"">
                                                                    <p style=""margin: 0 0 20px 0; font-size: 16px; color: #444444;"">Hey {user.UserName},</p>
                                                                    <p style=""margin: 0 0 20px 0; font-size: 16px; color: #444444;"">Super excited to have you join <span class=""highlight"" style=""color: #1E8449; font-weight: bold;"">WordWise</span>! You've just taken the first step towards building an amazing vocabulary.</p>
                                                                    <p style=""margin: 0 0 25px 0; font-size: 16px; color: #444444;"">To unlock everything WordWise has to offer and keep your progress safe, please verify your email by clicking the button below:</p>

                                                                    <!-- Call to Action Button -->
                                                                    <div class=""button-cta-container"" style=""text-align: center; padding: 10px 0 25px 0;"">
                                                                      <a href=""{confirmationLink}"" class=""button-cta"" style=""display:inline-block;padding:15px 40px;background-color:#FF9800;color:#ffffff !important;text-decoration:none;font-weight:bold;border-radius:30px;font-size:17px;text-align:center;border:none;cursor:pointer;"" target=""_blank"">
                                                                        Confirm My Account & Start Learning!
                                                                      </a>
                                                                    </div>

                                                                    <p style=""margin: 30px 0 15px 0; font-size: 16px; color: #444444;"">Once verified, you'll be all set to track your learning, get personalized recommendations, and receive important updates (like new features or password help if you ever need it!).</p>

                                                                    <p style=""margin: 30px 0 15px 0; font-size: 16px; color: #444444;"">Didn't sign up? No problem! You can safely ignore this email.</p>
                                                                    <p style=""margin: 0 0 15px 0; font-size: 16px; color: #444444;"">Let the word adventures begin!<br>- The WordWise Team</p>

                                                                    <!-- Fallback Link -->
                                                                    <div class=""link-alternative"" style=""font-size: 13px; color: #777777; margin-top: 25px; text-align: center; word-break: break-all;"">
                                                                        If the button isn't working, copy and paste this into your browser:
                                                                        <br>
                                                                        <a href=""{confirmationLink}"" target=""_blank"" style=""color: #FF9800 !important; text-decoration: underline; font-weight: 500;"">[Confirmation Link]</a>
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                        </table>

                                                        <!-- Footer Section -->
                                                        <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" style=""width:100%;"">
                                                            <tr>
                                                                <td align=""center"" class=""footer"" style=""padding: 30px 25px; text-align: center; font-size: 12px; color: #aaaaaa; font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif; background-color: #fcfcfc; border-top: 1px solid #f0f0f0;"">
                                                                    You received this email because an account was created for WordWise using this address.<br>
                                                                    <a href=""[Your Website Link]"" style=""color: #888888 !important; text-decoration: none; font-weight: 500;"">WordWiseApp.com</a> | 
                                                                    <a href=""[Privacy Policy Link]"" style=""color: #888888 !important; text-decoration: none; font-weight: 500;"">Privacy Policy</a> | 
                                                                    <a href=""[Support Link]"" style=""color: #888888 !important; text-decoration: none; font-weight: 500;"">Support</a>
                                                                    <br><br>
                                                                    © {DateTime.Now.Year.ToString()} WordWise. Happy Learning!
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                    <!--[if mso | IE]>
                                    </td>
                                    </tr>
                                    </table>
                                    <![endif]-->
                                </td>
                            </tr>
                        </table>
                    </body>
                    </html>
                ";
        }

        private string GenarateEmailResetPassword(string userName, string resetLink)
        {
            return $@"
            <!DOCTYPE html>
            <html lang=""en"">
            <head>
                <meta charset=""UTF-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                <title>Reset Your WordWise Password</title>
                <style>
                    /* Basic Reset */
                    body, table, td, a {{ -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; }}
                    table, td {{ mso-table-lspace: 0pt; mso-table-rspace: 0pt; }}
                    img {{ -ms-interpolation-mode: bicubic; border: 0; height: auto; line-height: 100%; outline: none; text-decoration: none; }}
                    table {{ border-collapse: collapse !important; }}
                    body {{ height: 100% !important; margin: 0 !important; padding: 0 !important; width: 100% !important; background-color: #f7f7f7; }}

                    /* Main Styles - Engaging & Clean */
                    .container {{
                        padding: 0;
                        max-width: 580px;
                        margin: 25px auto;
                        background-color: #ffffff;
                        border-radius: 12px;
                        font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif;
                        color: #333333;
                        line-height: 1.65;
                        box-shadow: 0 5px 20px rgba(0,0,0,0.07);
                        overflow: hidden;
                    }}
                    .header {{
                        background-color: #E8F5E9; /* Light Green */
                        padding: 40px 25px;
                        text-align: center;
                    }}
                    .header h1 {{
                        margin: 0;
                        color: #2E7D32; /* Vibrant Green */
                        font-size: 26px;
                        font-weight: bold;
                        line-height: 1.3;
                    }}
                     .header .sub-text {{
                         font-size: 16px;
                         color: #4CAF50;
                         margin-top: 8px;
                     }}
                    .content {{
                        padding: 40px 30px;
                    }}
                    .content p {{
                        margin: 0 0 20px 0;
                        font-size: 16px;
                        color: #444444;
                    }}
                    .content .highlight {{
                         color: #1E8449;
                         font-weight: bold;
                    }}
                    .important-note {{
                         font-size: 14px;
                         color: #555555;
                         background-color: #FFF9C4; /* Light Yellow for emphasis */
                         border-left: 4px solid #FFC107; /* Amber border */
                         padding: 10px 15px;
                         margin: 25px 0;
                         border-radius: 4px;
                    }}
                    .button-cta-container {{
                        text-align: center;
                        padding: 10px 0 25px 0;
                    }}
                    .button-cta {{
                        display: inline-block;
                        padding: 15px 40px;
                        background-color: #FF9800; /* Orange */
                        color: #ffffff !important;
                        text-decoration: none;
                        font-weight: bold;
                        border-radius: 30px;
                        font-size: 17px;
                        text-align: center;
                        border: none;
                        cursor: pointer;
                        transition: transform 0.1s ease, background-color 0.2s ease;
                    }}
                    .button-cta:hover {{
                         background-color: #FB8C00;
                         transform: scale(1.03);
                     }}
                    .link-alternative {{
                        font-size: 13px;
                        color: #777777;
                        margin-top: 25px;
                        text-align: center;
                        word-break: break-all;
                    }}
                     .link-alternative a {{
                        color: #FF9800 !important;
                        text-decoration: underline;
                        font-weight: 500;
                     }}
                    .footer {{
                        padding: 30px 25px;
                        text-align: center;
                        font-size: 12px;
                        color: #aaaaaa;
                        background-color: #fcfcfc;
                        border-top: 1px solid #f0f0f0;
                    }}
                     .footer a {{
                         color: #888888 !important;
                         text-decoration: none;
                         font-weight: 500;
                     }}
                     .footer a:hover {{
                         text-decoration: underline;
                     }}
                    /* Responsive Styles */
                    @media screen and (max-width: 600px) {{
                        body {{ font-size: 16px; }}
                        .container {{
                            width: 100% !important;
                            margin: 0 !important;
                            border-radius: 0 !important;
                            box-shadow: none !important;
                        }}
                        .header {{ padding: 35px 20px 30px 20px; }}
                        .header h1 {{ font-size: 24px; }}
                        .header .sub-text {{ font-size: 15px; }}
                        .content {{ padding: 30px 20px; }}
                        .content p {{ font-size: 15px; margin-bottom: 18px; }}
                        .important-note {{ font-size: 13px; padding: 8px 12px; }}
                        .button-cta {{ padding: 14px 30px; font-size: 16px; }}
                        .footer {{ padding: 25px 20px; font-size: 11px;}}
                    }}
                </style>
            </head>
            <body style=""margin: 0 !important; padding: 0 !important; background-color: #f7f7f7;"">
                <!-- Outer Table -->
                <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background-color: #f7f7f7;"">
                    <tr>
                        <td align=""center"" style=""padding: 10px;"">
                            <!-- Main Container -->
                            <!--[if mso | IE]>
                            <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""580"" align=""center"" style=""width:580px; background-color:#ffffff; border-radius: 12px;"">
                            <tr>
                            <td style=""line-height:0px;font-size:0px;mso-line-height-rule:exactly;"">
                            <![endif]-->
                            <div class=""container"" style=""max-width:580px;margin:25px auto;background:#ffffff;border-radius:12px;overflow:hidden;"">
                                <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" style=""font-size:0px;width:100%;background:#ffffff;border-radius:12px;"" align=""center"" border=""0"">
                                    <tbody>
                                        <tr>
                                            <td style=""vertical-align:top;padding:0;"">
                                                <!-- Header -->
                                                <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" style=""width:100%; background-color: #E8F5E9;"">
                                                    <tr>
                                                        <td align=""center"" style=""padding: 40px 25px;"">
                                                            <!-- Optional Logo -->
                                                            <!-- <img src=""[Your Logo URL]"" alt=""WordWise Logo"" width=""80"" style=""margin-bottom: 15px; border-radius: 8px;""> -->
                                                            <h1 style=""margin: 0; color: #2E7D32; font-size: 26px; font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif; font-weight: bold; line-height: 1.3;"">Let's Reset Your Password</h1>
                                                            <p class=""sub-text"" style=""font-size: 16px; color: #4CAF50; margin-top: 8px; font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif;"">We received a request for your WordWise account.</p>
                                                        </td>
                                                    </tr>
                                                </table>

                                                <!-- Content -->
                                                <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" style=""width:100%;"">
                                                    <tr>
                                                        <td align=""left"" style=""padding: 40px 30px; font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif;"">
                                                            <p style=""margin: 0 0 20px 0; font-size: 16px; color: #444444;"">Hi {userName},</p>
                                                            <p style=""margin: 0 0 20px 0; font-size: 16px; color: #444444;"">Forgot your password? No worries, it happens to the best of us! Click the button below to set up a new one for your <span class=""highlight"" style=""color: #1E8449; font-weight: bold;"">WordWise</span> account.</p>

                                                            <!-- Call to Action Button -->
                                                            <div class=""button-cta-container"" style=""text-align: center; padding: 10px 0 25px 0;"">
                                                              <a href=""{resetLink}"" class=""button-cta"" style=""display:inline-block;padding:15px 40px;background-color:#FF9800;color:#ffffff !important;text-decoration:none;font-weight:bold;border-radius:30px;font-size:17px;text-align:center;border:none;cursor:pointer;"" target=""_blank"">
                                                                Reset My Password
                                                              </a>
                                                            </div>

                                                            <!-- Important Security Note -->
                                                            <p class=""important-note"" style=""font-size: 14px; color: #555555; background-color: #FFF9C4; border-left: 4px solid #FFC107; padding: 10px 15px; margin: 25px 0; border-radius: 4px;"">
                                                                <strong>Heads up:</strong> This password reset link is only valid for the next <strong style=""color:#333;"">10 minutes</strong>. For security, please use it soon!
                                                            </p>

                                                            <p style=""margin: 30px 0 15px 0; font-size: 16px; color: #444444;"">If you <strong style=""color:#D32F2F;"">did not</strong> request a password reset, you can safely ignore this email. Your account is still secure.</p>

                                                            <!-- Fallback Link -->
                                                            <div class=""link-alternative"" style=""font-size: 13px; color: #777777; margin-top: 25px; text-align: center; word-break: break-all;"">
                                                                If the button doesn't work, copy and paste this link into your browser:
                                                                <br>
                                                                <a href=""{resetLink}"" target=""_blank"" style=""color: #FF9800 !important; text-decoration: underline; font-weight: 500;"">[Reset Link]</a>
                                                            </div>

                                                            <p style=""margin: 40px 0 15px 0; font-size: 16px; color: #444444;"">Stay sharp,<br>- The WordWise Team</p>

                                                        </td>
                                                    </tr>
                                                </table>

                                                <!-- Footer -->
                                                <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" style=""width:100%;"">
                                                    <tr>
                                                        <td align=""center"" class=""footer"" style=""padding: 30px 25px; text-align: center; font-size: 12px; color: #aaaaaa; font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif; background-color: #fcfcfc; border-top: 1px solid #f0f0f0;"">
                                                            If you have trouble, contact our support team at <a href=""mailto:[Support Email]"" style=""color: #888888 !important; text-decoration: underline; font-weight: 500;"">[Support Email]</a>.<br>
                                                            <a href=""[Your Website Link]"" style=""color: #888888 !important; text-decoration: none; font-weight: 500;"">WordWiseApp.com</a> | 
                                                            <a href=""[Privacy Policy Link]"" style=""color: #888888 !important; text-decoration: none; font-weight: 500;"">Privacy Policy</a>
                                                            <br><br>
                                                            © {DateTime.Now.Year.ToString()} WordWise.
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                            <!--[if mso | IE]>
                            </td>
                            </tr>
                            </table>
                            <![endif]-->
                        </td>
                    </tr>
                </table>
            </body>
            </html>
            ";
        }
    }
}
