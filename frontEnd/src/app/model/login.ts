export interface Login {
    loginID?: number;          // Maps to LoginID in the database and optional for frontend use
    email: string;            // Maps to Email
    passwordHash: string;     // Maps to PasswordHash
}