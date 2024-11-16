export interface Login {
    loginID: number;          // Maps to LoginID in the database
    email: string;            // Maps to Email
    passwordHash: string;     // Maps to PasswordHash
}