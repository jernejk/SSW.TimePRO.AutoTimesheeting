/// <reference types="Cypress" />

context('Gather data', () => {
    beforeEach(() => {
        // timepro-login.json is being excluded from git for privacy reasons.
        // It only requires email, password and host. (to support multiple environments and tenants)
        cy.clearCookies();
        cy.fixture('timepro-login.json')
          .then(json => {
            const loginData = json;
            cy.visit(loginData.host);

            cy.get('#Email')
              .type(loginData.email);
            
            cy.get('#Password')
              .type(loginData.password);

              cy.get('form[novalidate=novalidate]').submit();
        });
    });

    it('Should be logged in', () => {
        cy.get('a').contains('Timesheets').click();
        cy.get('a').contains('Week').click();
    });
});