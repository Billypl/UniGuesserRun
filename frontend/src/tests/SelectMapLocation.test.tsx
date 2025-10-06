import { render } from "@testing-library/react";

const mockUpMapEvents = jest.fn();

jest.mock("react-leaflet", () => ({
	useMapEvents: (events: any) => mockUpMapEvents(events),
}));

describe("SelectMapLocation component test", () => {
	it("calls selectLocationFunction on map click", () => {
		const mockSelectLocation  = jest.fn();
		const { SelectMapLocation } = require("../components/SelectMapLocation");
		
		render(
			<SelectMapLocation selectLocationFunction={mockSelectLocation} />
		);

		const selectCoordinates = {
			latlng: {
				lat: 51.505,
				lng: -0.09,
			},
		};

		expect(mockUpMapEvents).toHaveBeenCalled();

		const events = mockUpMapEvents.mock.calls[0][0];
		events.click(selectCoordinates);

		expect(mockSelectLocation ).toHaveBeenCalledWith({
			latitude: 51.505,
			longitude: -0.09,
		});
	});
});